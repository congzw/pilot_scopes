using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public class ClientMonitor : IClientMonitor
    {
        private readonly IClientConnectionRepository _repository;
        private readonly ClientInvokeProcessBus _clientInvokeProcessBus;
        private readonly ClientStubProcessBus _clientStubProcessBus;

        public ClientMonitor(IClientConnectionRepository connRepos
            , ClientInvokeProcessBus clientInvokeProcessBus
            , ClientStubProcessBus clientStubProcessBus)
        {
            _repository = connRepos ?? throw new ArgumentNullException(nameof(connRepos));
            _clientInvokeProcessBus = clientInvokeProcessBus ?? throw new ArgumentNullException(nameof(clientInvokeProcessBus));
            _clientStubProcessBus = clientStubProcessBus ?? throw new ArgumentNullException(nameof(clientStubProcessBus));
        }

        public static IDictionary<string, HubCallerContext> HubCallerContexts { get; set; } = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);

        public async Task OnConnected(OnConnectedEvent theEvent)
        {
            var hub = theEvent?.RaiseHub;
            if (hub == null)
            {
                //should not enter here! 
                throw new ArgumentException("hub should be init first!");
            }

            var locate = hub.TryGetClientConnectionLocate();
            if (string.IsNullOrWhiteSpace(locate.ClientId))
            {
                throw new ArgumentException("ClientId not find!");
            }
            if (string.IsNullOrWhiteSpace(locate.ScopeId))
            {
                throw new ArgumentException("ScopeId not find!");
            }
            if (string.IsNullOrWhiteSpace(locate.ConnectionId))
            {
                throw new ArgumentException("SignalRConnectionId not find!");
            }
            
            var connectionId = locate.ConnectionId;
            HubCallerContexts[connectionId] = hub.Context;

            if (locate.ClientId == HubConst.Monitor_ScopeId)
            {
                var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToFullName();
                await hub.Groups.AddToGroupAsync(connectionId, scopeGroup);
            }
            return;

            var theConn = _repository.GetConnection(locate);
            if (theConn != null)
            {
                //找到之前的记录，更新一个新的connectionId
                theConn.ConnectionId = connectionId;
            }
            else
            {
                //没有记录，新创建一个
                theConn = new MyConnection();
                var now = DateHelper.Instance.GetDateNow();
                theConn.ConnectionId = connectionId;
                theConn.CreateAt = now;
                theConn.LastUpdateAt = now;
                theConn.ScopeId = locate.ScopeId;
                theConn.ClientId = locate.ClientId;
                theConn.ConnectionId = connectionId;
                theConn.ClientType = hub.TryGetClientType();
                theConn.Bags.Add("access_token", "todo: refactor");
            }

            theConn.AddScopeGroupIfNotExist(ScopeGroupName.GetScopedGroupAll(locate.ScopeId));
            await theConn.UpdateConnectionGroups(hub);

            _repository.AddOrUpdate(theConn);

            await ReportToManageMonitor(hub.Clients, _repository, locate.ScopeId, locate.ClientId, nameof(OnConnected)).ConfigureAwait(false);
        }

        public async Task OnDisconnected(OnDisconnectedEvent theEvent)
        {
            return;
            var hub = theEvent?.RaiseHub;
            if (hub == null)
            {
                //should not enter here! 
                throw new InvalidOperationException("hub should be init first!");
            }

            var connectionId = hub.Context.ConnectionId;
            var conn = _repository.GetConnection(ClientConnectionLocate.Create().WithConnectionId(connectionId));
            if (conn == null)
            {
                //find no conn, should never enter here
                return;
            }

            //不删除myConnection数据，仅仅把connectionId设置为string.empty，标识当前为掉线状态
            conn.ConnectionId = string.Empty;
            conn.LastUpdateAt = DateHelper.Instance.GetDateNow();
            _repository.AddOrUpdate(conn);

            await ReportToManageMonitor(hub.Clients, _repository, conn.ScopeId, conn.ClientId, nameof(OnDisconnected)).ConfigureAwait(false);
        }

        public async Task KickClient(KickClientEvent theEvent)
        {
            return;
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));
            var args = theEvent.Args;
            if (args == null)
            {
                return;
            }

            var hub = theEvent.RaiseHub;
            if (hub != null)
            {
                await Kick(hub.Clients, hub.Groups, HubCallerContexts, _repository, args).ConfigureAwait(false);
            }
            else
            {
                var wrapper = theEvent.Context;
                await Kick(wrapper.Clients, wrapper.Groups, HubCallerContexts, _repository, args).ConfigureAwait(false);
            }
            await ReportToManageMonitor(theEvent.TryGetHubClients(), _repository, args.ScopeId, args.ClientId, "OnKick").ConfigureAwait(false);
        }

        public async Task ClientInvoke(ClientInvokeEvent theEvent)
        {
            if (theEvent == null)
            {
                return;
            }
            await _clientInvokeProcessBus.Process(theEvent).ConfigureAwait(false);
            //todo
            //if (theEvent.StopSendToAll)
            //{
            //    return;
            //}

            var now = DateHelper.Instance.GetDateNow();
            var args = theEvent.Args;
            var hubCallerClients = theEvent.TryGetHubClients();
            //if (args.ToClientIds.Any())
            //{
            //    //todo
            //    //var locates = new List<IClientConnectionLocate>();
            //    //foreach (var toClientId in args.ToClientIds)
            //    //{
            //    //    var locate = new ClientConnectionLocate()
            //    //    {
            //    //        ClientId = toClientId,
            //    //        ScopeId = args.ScopeId
            //    //    };
            //    //    locates.Add(locate);
            //    //}

            //    //var theConnections = _repository.GetConnections(locates).ToList();
            //    //foreach (var theConnection in theConnections)
            //    //{
            //    //    theConnection.LastUpdateAt = now;
            //    //}
            //    //var connectionIds = theConnections.Select(x => x.ConnectionId).ToList();
            //    //await hubCallerClients.Clients(connectionIds).SendAsync(HubConst.ClientInvoke, args);
            //}
            //else
            //{
            //    var theConnections = _repository.GetConnections(args).ToList();
            //    foreach (var theConnection in theConnections)
            //    {
            //        theConnection.LastUpdateAt = now;
            //    }
            //    await hubCallerClients.All.SendAsync(HubConst.ClientInvoke, args);
            //}

            await ReportToManageMonitor(hubCallerClients, _repository, args.ScopeId, args.ClientId, nameof(ClientInvoke)).ConfigureAwait(false);
        }

        public async Task ClientStub(ClientStubEvent theEvent)
        {
            var hubContext = theEvent.Context;
            if (hubContext == null)
            {
                throw new ArgumentException("Hub context is null");
            }
            await _clientStubProcessBus.Process(theEvent).ConfigureAwait(false);
            
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "ClientStub", JsonConvert.SerializeObject(theEvent.Args, Formatting.None)));
            ////todo
            //if (theEvent.StopSendToAll)
            //{
            //    return;
            //}

            var now = DateHelper.Instance.GetDateNow();
            var args = theEvent.Args;
            //if (args.ToClientIds.Any())
            //{
            //    //todo
            //    //var locates = new List<IClientConnectionLocate>();
            //    //foreach (var toClientId in args.ToClientIds)
            //    //{
            //    //    var locate = new ClientConnectionLocate()
            //    //    {
            //    //        ClientId = toClientId,
            //    //        ScopeId = args.ScopeId
            //    //    };

            //    //    locates.Add(locate);
            //    //}

            //    //var theConnections = _repository.GetConnections(locates).ToList();
            //    //foreach (var theConnection in theConnections)
            //    //{
            //    //    theConnection.LastUpdateAt = now;
            //    //}

            //    //var connectionIds = theConnections.Select(x => x.ConnectionId).ToList();
            //    //await hubContext.Clients.Clients(connectionIds).SendAsync(HubConst.ClientStub, args);
            //}
            //else
            //{
            //    //todo: update conn
            //    //var theConnections = _repository.Query().ToList();
            //    //foreach (var theConnection in theConnections)
            //    //{
            //    //    theConnection.LastUpdateAt = now;
            //    //}
            //    await hubContext.Clients.All.SendAsync(HubConst.ClientStub, args);
            //}

            var hubCallerClients = theEvent.TryGetHubClients();
            await ReportToManageMonitor(hubCallerClients, _repository, args.ScopeId, args.ClientId, nameof(ClientStub)).ConfigureAwait(false);
        }

        public Task<IList<ScopeGroup>> GetGroups(IScopeGroupLocate args)
        {
            throw new System.NotImplementedException();
        }

        public Task<ScopeGroup> GetGroup(IScopeGroupLocate args)
        {
            throw new System.NotImplementedException();
        }

        public Task AddGroup(AddGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveGroup(RemoveGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task AddToGroup(AddToGroupArgs args)
        {
            var theConn = _repository.GetConnection(new ClientConnectionLocate());
            throw new System.NotImplementedException();
        }

        public Task RemoveFromGroup(RemoveFromGroupArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<ScopeClientGroup>> GetGroups(IScopeKey args)
        {
            throw new System.NotImplementedException();
        }

        public Task ResetScope(ResetScopeEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateScope(UpdateScopeEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        private static async Task Kick(IHubClients<IClientProxy> hubClients,
            IGroupManager groupManager,
            IDictionary<string, HubCallerContext> hubCallerContexts,
            IClientConnectionRepository repository,
            KickClientArgs args)
        {
            var theConn = repository.GetConnection(args);
            if (theConn == null)
            {
                return;
            }
            theConn.LastUpdateAt = DateHelper.Instance.GetDateNow();

            var clientProxy = hubClients.Client(theConn.ConnectionId);
            if (clientProxy == null)
            {
                return;
            }

            //notify kick!
            var clientMethodArgs = ClientMethodArgs
                .Create(HubConst.ClientMethod_Notify)
                .SetBagValue("args", args);

            await clientProxy.SendAsync(HubConst.ClientStub, clientMethodArgs).ConfigureAwait(false);

            //todo
            ////kick old connectionId
            //foreach (var scopedGroup in theConn.Groups)
            //{
            //    await groupManager.RemoveFromGroupAsync(theConn.ConnectionId, scopedGroup).ConfigureAwait(false);
            //    await hubClients.Group(scopedGroup).SendAsync(HubConst.ClientStub_RemoveFromGroupStubInvoke, string.Format("{0}离开了本小组", theConn.ToLocateDesc()));
            //}
            repository.Remove(theConn);

            hubCallerContexts.TryGetValue(theConn.ConnectionId, out var oldClientHub);
            oldClientHub?.Abort();
        }

        private static Task ReportToManageMonitor(
            IHubClients<IClientProxy> hubClients,
            IClientConnectionRepository repository,
            string invokeScopeId,
            string invokeClientId,
            string invokeDesc)
        {
            return Task.CompletedTask;
            //report to monitor, if necessary
            var info = new MonitorInvokeInfo();
            info.ScopeId = invokeScopeId;
            info.ClientId = invokeClientId;
            info.Desc = invokeDesc;

            return ManageMonitorHelper.Instance.UpdateMonitorInfo(hubClients, info);
        }
    }
}
