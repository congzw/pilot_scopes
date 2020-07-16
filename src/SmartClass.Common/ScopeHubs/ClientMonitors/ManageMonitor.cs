using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public class EventInvokeInfo
    {
        public string Desc { get; set; }
        public string ConnectionId { get; set; }
        public SendContext SendContext { get; set; }
        public DateTime InvokeAt { get; set; } = DateHelper.Instance.GetDateNow();
    }

    public class UpdateConnectionsArgs
    {
        public int OnlineCount { get; set; }
        public int OfflineCount { get; set; }
        public int TotalCount { get; set; }
        public IList<MyConnection> Connections { get; set; } = new List<MyConnection>();

        public static UpdateConnectionsArgs Create(IList<MyConnection> connections)
        {
            var updateConnectionsArgs = new UpdateConnectionsArgs();
            updateConnectionsArgs.Connections = connections;
            updateConnectionsArgs.TotalCount = connections.Count;
            updateConnectionsArgs.OnlineCount = connections.Count(x => !string.IsNullOrWhiteSpace(x.ConnectionId));
            updateConnectionsArgs.OfflineCount = updateConnectionsArgs.TotalCount - updateConnectionsArgs.OnlineCount;
            return updateConnectionsArgs;
        }
    }

    public class UpdateClientTreeArgs
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int Value { get; set; } //TotalConnectionCount
        
        private readonly List<UpdateClientTreeArgs> _children = new List<UpdateClientTreeArgs>();
        public IReadOnlyList<UpdateClientTreeArgs> Children => _children;

        public UpdateClientTreeArgs GetOrCreateChild(string type, string name, int value)
        {
            var theOne = Children.FirstOrDefault(x => x.Name.MyEquals(name));
            if (theOne != null)
            {
                return theOne;
            }

            var child = new UpdateClientTreeArgs { Type = type, Name = name, Value = value };
            _children.Add(child);
            return child;
        }
        
        private static string Root = "Root";
        private static string Scope = "Scope";
        private static string Group = "Group";
        private static string Client = "Client";
        private static string Connection = "Connection";

        public static UpdateClientTreeArgs Create(IList<ScopeClientGroup> scopeClientGroups, IList<MyConnection> connections)
        {
            var scopeConnections = connections.GroupBy(x => x.ScopeId).ToList();
            var rootNode = new UpdateClientTreeArgs { Type = Root, Name = "PV100", Value = connections.Count };

            foreach (var scopeConnection in scopeConnections)
            {
                var myConnections = scopeConnection.ToList();
                var scopeNode = rootNode.GetOrCreateChild(Scope, scopeConnection.Key, myConnections.Count);

                foreach (var connection in myConnections)
                {
                    //check if not in any group
                    var count = scopeClientGroups.Count(x => x.ClientId.MyEquals(connection.ClientId));
                    if (count == 0)
                    {
                        var singleClientNode = scopeNode.GetOrCreateChild(Client, connection.ClientId, 1);
                        singleClientNode.GetOrCreateChild(Connection, connection.ConnectionId, 0);
                    }
                }
            }

            var scopes = scopeClientGroups.GroupBy(x => x.ScopeId).ToList();
            foreach (var scope in scopes)
            {
                var groups = scope.GroupBy(x => x.Group).ToList();
                var scopeNode = rootNode.GetOrCreateChild(Scope, scope.Key, groups.Count);
                foreach (var @group in groups)
                {
                    var clients = @group.GroupBy(x => x.ClientId).ToList();
                    var groupNode = scopeNode.GetOrCreateChild(Group, group.Key, clients.Count);
                    foreach (var client in clients)
                    {
                        var clientNode = groupNode.GetOrCreateChild(Client, client.Key, 1);
                        var scopeClientGroup = client.FirstOrDefault();
                        if (scopeClientGroup != null)
                        {
                            var theOne = connections.SingleOrDefault(x =>
                                x.GetScopeClientKey().MyEquals(scopeClientGroup.GetScopeClientKey()));
                            if (theOne != null)
                            {
                                clientNode.GetOrCreateChild(Connection, theOne.ConnectionId, 0);
                            }
                        }
                    }
                }
            }
            
            return rootNode;
        }
    }

    public class ServerLogInfo
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public object Details { get; set; }

        public static ServerLogInfo Create(string message, object details = null, string category = "")
        {
            return new ServerLogInfo() { Category = "[ServerLog]" + category, Message = message, Details = details };
        }
    }

    public class ManageMonitorHelper
    {
        public ManageMonitorHelper()
        {
            //todo with a api
            Config.EventInvokeInfoEnabled = true;
            Config.UpdateConnectionsEnabled = true;
            Config.ServerLogEnabled = true;
        }

        public ManageMonitorConfig Config { get; set; } = new ManageMonitorConfig();
        
        public Task EventInvoked(IHubClients<IClientProxy> hubClients, EventInvokeInfo info)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_EventInvoked, info);
        }

        public Task ServerLog(IHubClients<IClientProxy> hubClients, ServerLogInfo logInfo)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_ServerLog, logInfo);
        }
        
        public Task TraceSignalREvent(SignalREvent theEvent, string descAppend)
        {
            if (!Config.EventInvokeInfoEnabled)
            {
                return Task.CompletedTask;
            }
            var hubClients = theEvent.TryGetHubClients();
            var eventName = theEvent.GetType().Name;
            var info = new EventInvokeInfo();
            info.SendContext = theEvent.SendContext;
            info.Desc = eventName + descAppend;
            info.ConnectionId = theEvent.RaiseHub?.Context?.ConnectionId;

            return EventInvoked(hubClients, info);
        }

        public async Task UpdateMonitor(SignalREvent theEvent, IClientConnectionRepository _connectionRepository, IScopeClientGroupRepository _scopeClientGroupRepository)
        {
            if (Config.UpdateConnectionsEnabled)
            {
                var connections = _connectionRepository.GetConnections(new GetConnectionsArgs());
                var scopeClientGroups = _scopeClientGroupRepository.GetScopeClientGroups(new ScopeClientGroupLocate());

                var hubClients = theEvent.TryGetHubClients();
                var updateConnectionsArgs = UpdateConnectionsArgs.Create(connections);
                await UpdateConnections(hubClients, updateConnectionsArgs);

                var updateClientTreeArgs = UpdateClientTreeArgs.Create(scopeClientGroups, connections);
                await UpdateClientTree(hubClients, updateClientTreeArgs);
            }
        }
        
        private Task UpdateConnections(IHubClients<IClientProxy> hubClients, UpdateConnectionsArgs args)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_UpdateConnections, args);
        }

        private Task UpdateClientTree(IHubClients<IClientProxy> hubClients, UpdateClientTreeArgs args)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_UpdateClientTree, args);
        }

        public static ManageMonitorHelper Instance = new ManageMonitorHelper();
    }

    public class ManageMonitorConfig
    {
        /// <summary>
        /// 指示是否由服务器端主动推送连接状态信息到Monitor
        /// 客户端可以选择主动接受也可以被动接受
        /// </summary>
        public bool EventInvokeInfoEnabled { get; set; }

        /// <summary>
        /// 是否包含连接列表
        /// </summary>
        public bool UpdateConnectionsEnabled { get; set; }

        /// <summary>
        /// 是否包含服务器端的日志
        /// </summary>
        public bool ServerLogEnabled { get; set; }
    }
}
