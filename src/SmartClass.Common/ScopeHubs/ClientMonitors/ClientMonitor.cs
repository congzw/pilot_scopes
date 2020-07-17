using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public class ClientMonitor : IClientMonitor
    {
        private readonly IClientConnectionRepository _repository;
        private readonly IScopeClientGroupRepository _clientGroupRepos;
        private readonly ClientMethodProcessBus _clientMethodProcessBus;
        private readonly HubCallerContextCache _hubCallerContextCache;
        private readonly ScopeClientConnectionKeyMaps _scopeClientConnectionKeyMaps;
        private readonly SignalRConnectionCache _signalRConnectionCache;

        public ClientMonitor(IClientConnectionRepository connRepos
            , IScopeClientGroupRepository clientGroupRepos
            , ClientMethodProcessBus clientMethodProcessBus
            , HubCallerContextCache hubCallerContextCache
            , ScopeClientConnectionKeyMaps scopeClientConnectionKeyMaps
            , SignalRConnectionCache signalRConnectionCache)
        {
            _repository = connRepos ?? throw new ArgumentNullException(nameof(connRepos));
            _clientGroupRepos = clientGroupRepos ?? throw new ArgumentNullException(nameof(clientGroupRepos));
            _clientMethodProcessBus = clientMethodProcessBus ?? throw new ArgumentNullException(nameof(clientMethodProcessBus));
            _hubCallerContextCache = hubCallerContextCache ?? throw new ArgumentNullException(nameof(hubCallerContextCache));
            _scopeClientConnectionKeyMaps = scopeClientConnectionKeyMaps ?? throw new ArgumentNullException(nameof(scopeClientConnectionKeyMaps));
            _signalRConnectionCache = signalRConnectionCache;
        }
        
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

            var signalRConnection = new SignalRConnection().WithScopeId(locate.ScopeId).WithClientId(locate.ClientId).WithConnectionId(locate.ConnectionId);
            _signalRConnectionCache.Set(signalRConnection);

            var theCallerContext = TryGetHubCallerContext(locate);
            if (theCallerContext != null)
            {
                //为了保持Scope+Client的唯一通道，踢掉原有的连接通道！目前客户端有重连逻辑，如果服务器端断开，会导致死循环！改为通知客户端自己处理
                //theCallerContext.Abort();
                var clientMethodArgs = ClientMethodArgs.Create(HubConst.ClientMethod_Kicked);
                clientMethodArgs.MethodArgs = new { Reason = "Same Scope Client Connected: " + locate.GetScopeClientKey()};
                await hub.Clients.Client(theCallerContext.ConnectionId).SendAsync(HubConst.ClientMethod, clientMethodArgs);

                //trace for manage
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
                //var oldConnId = theConn.ConnectionId;
                //todo: 老的连接是暂不处理
                //保证原有的连接组，使用新的ConnectionId被重建一遍
                var getClientGroupsArgs = new GetClientGroupsArgs().WithScopeId(locate.ScopeId).WithClientId(locate.ClientId);
                var scopeClientGroups = _clientGroupRepos.GetScopeClientGroups(getClientGroupsArgs);
                foreach (var scopeClientGroup in scopeClientGroups)
                {
                    var groupManager = hub.Groups;
                    var scopeGroupFullName = ScopeGroupName.GetScopedGroup(scopeClientGroup.ScopeId, scopeClientGroup.Group).ToScopeGroupFullName();
                    await groupManager.AddToGroupAsync(locate.ConnectionId, scopeGroupFullName).ConfigureAwait(false);
                }

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
            }

            _repository.AddOrUpdate(theConn);

            //set scopeContext
            var scopeContext = ScopeContext.GetScopeContext(locate.ScopeId);
            scopeContext.OnConnected(locate);
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

            //set scopeContext
            var scopeContext = ScopeContext.GetScopeContext(locate.ScopeId);
            scopeContext.OnDisconnected(locate);
            
            _signalRConnectionCache.Remove(locate.ConnectionId);
        }
        
        public async Task ClientMethod(ClientMethodEvent theEvent)
        {
            //可能来自Hub的内部和外部的请求
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));

            //默认在自身连接的ScopeId内广播
            theEvent.SendContext.AutoFixToGroupAllIfEmpty();

            await _clientMethodProcessBus.Process(theEvent).ConfigureAwait(false);
            await SendClientMethod(theEvent, theEvent.Args, HubConst.ClientMethod).ConfigureAwait(false);
        }
        
        public async Task JoinGroup(JoinGroupEvent theEvent)
        {
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));
            var args = theEvent.Args;
            if (args == null) throw new ArgumentNullException(nameof(args));
            
            foreach (var clientId in args.ClientIds)
            {
                var connectionLocate = ClientConnectionLocate.Create().WithScopeId(args.ScopeId).WithClientId(clientId);
                var theConn = _repository.GetConnection(connectionLocate);
                if (theConn != null)
                {
                    //theConn.AddScopeGroupIfNotExist(args);
                    var groupManager = theEvent.TryGetGroupManager();
                    var groupName = args.ToScopeGroupFullName();
                    await groupManager.AddToGroupAsync(theConn.ConnectionId, groupName).ConfigureAwait(false);
                }
                
                var scopeClientGroup = new ScopeClientGroup().WithScopeId(args.ScopeId).WithGroup(args.Group).WithClientId(clientId);
                _clientGroupRepos.Add(scopeClientGroup);
            }
        }

        public async Task LeaveGroup(LeaveGroupEvent theEvent)
        {
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));
            var args = theEvent.Args;
            if (args == null) throw new ArgumentNullException(nameof(args));

            foreach (var clientId in args.ClientIds)
            {
                var connectionLocate = ClientConnectionLocate.Create().WithScopeId(args.ScopeId).WithClientId(clientId);
                var theConn = _repository.GetConnection(connectionLocate);
                if (theConn != null)
                {
                    var groupManager = theEvent.TryGetGroupManager();
                    var groupName = args.ToScopeGroupFullName();
                    await groupManager.RemoveFromGroupAsync(theConn.ConnectionId, groupName).ConfigureAwait(false);
                }
                
                var locate = new ScopeClientGroup().WithScopeId(args.ScopeId).WithGroup(args.Group).WithClientId(clientId);
                _clientGroupRepos.Remove(locate);
            }
        }

        public Task<IList<ScopeClientGroup>> GetClientGroups(GetClientGroupsArgs args)
        {
            var scopeClientGroups = _clientGroupRepos.GetScopeClientGroups(args);
            return Task.FromResult(scopeClientGroups);
        }

        public async Task ResetScope(ResetScopeEvent theEvent)
        {
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));

            var resetScopeArgs = theEvent.Args;
            if (resetScopeArgs == null || string.IsNullOrWhiteSpace(resetScopeArgs.ScopeId))
            {
                throw new ArgumentException("scopeId should have value!");
            }

            //clear all cache, remove all group relation, empty all connId, clear scope context
            var scopeClientGroups = _clientGroupRepos.GetScopeClientGroups(new ScopeClientGroupLocate().WithScopeId(resetScopeArgs.ScopeId));
            foreach (var scopeClientGroup in scopeClientGroups)
            {
                var locate = _scopeClientConnectionKeyMaps.TryGetByScopeClientKey(scopeClientGroup.ScopeId,
                    scopeClientGroup.ClientId);
                if (locate != null)
                {
                    var groupManager = theEvent.TryGetGroupManager();
                    var groupFullName = ScopeGroupName.GetScopedGroup(scopeClientGroup.ScopeId, scopeClientGroup.Group)
                        .ToScopeGroupFullName();
                    await groupManager.RemoveFromGroupAsync(locate.ConnectionId, groupFullName);
                    _scopeClientConnectionKeyMaps.RemoveCache(locate);
                }

                _clientGroupRepos.Remove(scopeClientGroup);
            }

            var connections = _repository.GetConnections(new GetConnectionsArgs().WithScopeId(resetScopeArgs.ScopeId));
            foreach (var myConnection in connections)
            {
                myConnection.ConnectionId = string.Empty;
                _repository.AddOrUpdate(myConnection);
            }

            var scopeContext = ScopeContext.GetScopeContext(resetScopeArgs.ScopeId, true);
            scopeContext.Bags.Clear();
        }

        public Task UpdateScope(UpdateScopeEvent theEvent)
        {
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));

            var resetScopeArgs = theEvent.Args;
            if (resetScopeArgs == null || string.IsNullOrWhiteSpace(resetScopeArgs.ScopeId))
            {
                throw new ArgumentException("scopeId should have value!");
            }

            var scopeContext = ScopeContext.GetScopeContext(resetScopeArgs.ScopeId, true);
            foreach (var bag in resetScopeArgs.Bags)
            {
                scopeContext.SetBagValue(bag.Key, bag.Value);
            }
            return Task.CompletedTask;
        }

        public Task<IList<ScopeContext>> GetScopeContexts()
        {
            var scopeRepository = ScopeContext.Resolve();
            var scopeContexts = scopeRepository.GetScopeContexts();
            return Task.FromResult(scopeContexts);
        }

        //helpers
        private static IClientConnectionLocate AllPropsShouldHasValue(IClientConnectionLocate locate)
        {
            if (string.IsNullOrWhiteSpace(locate.ClientId))
            {
                throw new ArgumentException("ClientId should has value!");
            }
            if (string.IsNullOrWhiteSpace(locate.ScopeId))
            {
                throw new ArgumentException("ScopeId should has value!");
            }
            if (string.IsNullOrWhiteSpace(locate.ConnectionId))
            {
                throw new ArgumentException("SignalR ConnectionId should has value!");
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

            //是否需要去重复？ GroupName -> ClientIds
            var fullNameGroups = sendTo.GetFullNameGroups();
            if (!fullNameGroups.IsNullOrEmpty())
            {
                await hubCallerClients.Groups(fullNameGroups).SendAsync(clientMethod, clientMethodArgs);
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
    }
}
