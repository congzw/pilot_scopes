using System;
using System.Linq;
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
        private readonly HubCallerContextCache _hubCallerContextCache;
        private readonly ScopeClientConnectionKeyMaps _scopeClientConnectionKeyMaps;

        public ClientMonitor(IClientConnectionRepository connRepos
            , ClientInvokeProcessBus clientInvokeProcessBus
            , ClientStubProcessBus clientStubProcessBus
            , HubCallerContextCache hubCallerContextCache
            , ScopeClientConnectionKeyMaps scopeClientConnectionKeyMaps)
        {
            _repository = connRepos ?? throw new ArgumentNullException(nameof(connRepos));
            _clientInvokeProcessBus = clientInvokeProcessBus ?? throw new ArgumentNullException(nameof(clientInvokeProcessBus));
            _clientStubProcessBus = clientStubProcessBus ?? throw new ArgumentNullException(nameof(clientStubProcessBus));
            _hubCallerContextCache = hubCallerContextCache;
            _scopeClientConnectionKeyMaps = scopeClientConnectionKeyMaps;
        }
        
        private static object _lock = new object();

        public async Task OnConnected(OnConnectedEvent theEvent)
        {
            var hub = theEvent?.RaiseHub;
            if (hub == null)
            {
                //should not enter here! 
                throw new ArgumentException("hub should be init first!");
            }

            var sendFrom = hub.TryGetHttpContext().GetSendFrom();
            var locate = hub.TryGetClientConnectionLocate();
            AllPropsShouldHasValue(locate);
            
            var theCallerContext = TryGetHubCallerContext(locate);
            if (theCallerContext != null)
            {
                //为了保持Scope+Client的唯一通道，踢掉原有的连接通道！目前客户端有重连逻辑，如果服务器端断开，会导致死循环！改为通知客户端自己处理
                //theCallerContext.Abort();
                var clientMethodArgs = ClientMethodArgs.Create(HubConst.ClientMethod_Kicked);
                clientMethodArgs.MethodArgs = new { Reason = "Same Scope Client Connected: " + locate.GetScopeClientKey()};
                await hub.Clients.Client(theCallerContext.ConnectionId).SendAsync(HubConst.ClientStub, clientMethodArgs);

                var hubClients = theEvent.TryGetHubClients();
                var eventName = theEvent.GetType().Name;
                var info = new EventInvokeInfo();
                info.SendContext = theEvent.SendContext;
                info.Desc = eventName + " same client kick";
                info.ConnectionId = theCallerContext.ConnectionId;
                await ManageMonitorHelper.Instance.EventInvoked(hubClients, info);
            }

            _hubCallerContextCache.SetCache(locate.ConnectionId, hub.Context);
            _scopeClientConnectionKeyMaps.SetCache(locate);
            await ScopeGroupFix.OnConnected(hub, locate.ScopeId);

            var theConn = _repository.GetConnection(locate);
            if (theConn != null)
            {
                //找到之前的记录，更新一个新的connectionId
                theConn.ConnectionId = locate.ConnectionId;
            }
            else
            {
                //没有记录，新创建一个
                theConn = new MyConnection();
                var now = DateHelper.Instance.GetDateNow();
                theConn.ScopeId = locate.ScopeId;
                theConn.ClientId = locate.ClientId;
                theConn.ConnectionId = locate.ConnectionId;
                theConn.CreateAt = now;
                theConn.LastUpdateAt = now;
                theConn.ClientType = sendFrom.ClientType;
                //theConn.Bags.Add("access_token", "todo: refactor");
                
                theConn.AddScopeGroupIfNotExist(ScopeGroupName.GetScopedGroupAll(locate.ScopeId));
                await theConn.UpdateConnectionGroups(hub);
            }

            _repository.AddOrUpdate(theConn);
        }

        public async Task OnDisconnected(OnDisconnectedEvent theEvent)
        {
            var hub = theEvent?.RaiseHub;
            if (hub == null)
            {
                //should not enter here! 
                throw new InvalidOperationException("hub should be init first!");
            }

            var locate = hub.TryGetClientConnectionLocate();
            AllPropsShouldHasValue(locate);
            
            var theCallerContext = TryGetHubCallerContext(locate);
            if (theCallerContext != null)
            {
                _hubCallerContextCache.RemoveCache(locate.ConnectionId);
            }

            await ScopeGroupFix.OnDisconnected(hub, locate.ScopeId);
            var conn = _repository.GetConnection(locate);
            if (conn == null)
            {
                //find no conn, should never enter here
                return;
            }

            //不删除myConnection数据，仅仅把connectionId设置为string.empty，标识当前为掉线状态
            conn.ConnectionId = string.Empty;
            conn.LastUpdateAt = DateHelper.Instance.GetDateNow();
            _repository.AddOrUpdate(conn);
        }

        private HubCallerContext TryGetHubCallerContext(ClientConnectionLocate locate)
        {
            //是否已经建立了通道
            var theClientConnection = _scopeClientConnectionKeyMaps.TryGetByScopeClientKey(locate.ScopeId, locate.ClientId);
            if (theClientConnection == null)
            {
                return null;
            }

            var theCallerContext = _hubCallerContextCache.GetCache(theClientConnection.ConnectionId);
            return theCallerContext;
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
                await Kick(hub.Clients, hub.Groups, _hubCallerContextCache.HubCallerContexts, _repository, args).ConfigureAwait(false);
            }
            else
            {
                var wrapper = theEvent.Context;
                await Kick(wrapper.Clients, wrapper.Groups, _hubCallerContextCache.HubCallerContexts, _repository, args).ConfigureAwait(false);
            }
        }

        public async Task ClientInvoke(ClientInvokeEvent theEvent)
        {
            //来自Hub的请求
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));
            var raiseHub = theEvent.RaiseHub ?? throw new ArgumentException("ClientInvoke方法只能用于基于Hub连接的客户端请求");
            var sendFrom = raiseHub.GetSendFrom();
            //基于hub的invoke，默认在自身连接的ScopeId内广播
            if (string.IsNullOrWhiteSpace(theEvent.SendContext.To.ScopeId))
            {
                theEvent.SendContext.To = SendTo.CreateForScopeGroupAll(sendFrom.ScopeId);
            }

            await _clientInvokeProcessBus.Process(theEvent).ConfigureAwait(false);
            await SendClientMethod(theEvent, theEvent.Args, HubConst.ClientInvoke).ConfigureAwait(false);
        }

        public async Task ClientStub(ClientStubEvent theEvent)
        {
            //来自API的请求
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));
            await _clientStubProcessBus.Process(theEvent).ConfigureAwait(false);
            await SendClientMethod(theEvent, theEvent.Args, HubConst.ClientStub).ConfigureAwait(false);
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

        public Task JoinGroup(JoinGroupArgs args)
        {
            var theConn = _repository.GetConnection(new ClientConnectionLocate());
            throw new System.NotImplementedException();
        }

        public Task LeaveGroup(LeaveGroupArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<ScopeClientGroup>> GetClientGroups(GetClientGroupsArgs args)
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
        
        //helpers
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
        private static IClientConnectionLocate AllPropsShouldHasValue(IClientConnectionLocate locate)
        {
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

            return locate;
        }
        private async Task SendClientMethod(SignalREvent theEvent, ClientMethodArgs clientMethodArgs, string clientMethod)
        {
            if (theEvent.StopSend)
            {
                return;
            }

            var sendArgs = theEvent.SendContext;

            var sendTo = sendArgs?.To;
            if (sendTo == null)
            {
                return;
            }

            var hubCallerClients = theEvent.TryGetHubClients();
            foreach (var group in sendTo.Groups)
            {
                var groupFullName = ScopeGroupName.GetScopedGroup(sendTo.ScopeId, @group).ToScopeGroupFullName();
                await hubCallerClients.Groups(groupFullName).SendAsync(clientMethod, clientMethodArgs);
            }

            var connectionIds = new List<string>();
            foreach (var toClientId in sendTo.ClientIds)
            {
                var clientConnectionLocate = _scopeClientConnectionKeyMaps.TryGetByScopeClientKey(sendTo.ScopeId, toClientId);
                if (clientConnectionLocate != null)
                {
                    connectionIds.Add(clientConnectionLocate.ConnectionId);
                }
            }

            if (!connectionIds.IsNullOrEmpty())
            {
                await hubCallerClients.Clients(connectionIds).SendAsync(clientMethod, clientMethodArgs);
            }
        }
    }
}
