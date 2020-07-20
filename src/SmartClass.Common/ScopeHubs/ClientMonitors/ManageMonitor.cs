using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

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
                theOne.Value = value;
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

        public static UpdateClientTreeArgs Create(IList<ScopeClientGroup> relations, IList<SignalRConnection> signalRConnections, IList<MyConnection> clientConnections)
        {
            var rootNode = new UpdateClientTreeArgs { Type = Root, Name = "PV100", Value = signalRConnections.Count };

            var gScopes = clientConnections.GroupBy(x => x.ScopeId).ToList();
            foreach (var gScope in gScopes)
            {
                //即使没有任何活动的连接，也为scope造型
                var scopeNode = rootNode.GetOrCreateChild(Scope, gScope.Key, 0);

                var gConnections = gScope.ToList();
                foreach (var myConnection in gConnections)
                {
                    //check if not in any group
                    var count = relations.Count(x => x.ClientId.MyEquals(myConnection.ClientId) && x.ScopeId.MyEquals(myConnection.ScopeId));
                    if (count == 0)
                    {
                        //即使没有任何活动的连接，也为不属于group的client和conn造型
                        var singleClientNode = scopeNode.GetOrCreateChild(Client, myConnection.ClientId, 0);
                        var singleClientConnNode = singleClientNode.GetOrCreateChild(Connection, myConnection.ConnectionId, 0);
                    }
                }
            }

            //signalRConnections
            var theConnScopes = signalRConnections.GroupBy(x => x.ScopeId).ToList();
            foreach (var theConnScope in theConnScopes)
            {
                var scopeNode = rootNode.GetOrCreateChild(Scope, theConnScope.Key, theConnScope.Count());
                foreach (var connection in theConnScope)
                {
                    //check if not in any group
                    var count = relations.Count(x => x.ClientId.MyEquals(connection.ClientId));
                    if (count == 0)
                    {
                        var singleClientNode = scopeNode.GetOrCreateChild(Client, connection.ClientId, 1);
                        singleClientNode.GetOrCreateChild(Connection, connection.ConnectionId, 0);
                    }
                }
            }

            var theScopes = relations.GroupBy(x => x.ScopeId).ToList();
            foreach (var theScope in theScopes)
            {
                //root->scopes
                var theGroups = theScope.GroupBy(x => x.Group).ToList();
                var scopeNode = rootNode.GetOrCreateChild(Scope, theScope.Key, theGroups.Count);
                foreach (var theGroup in theGroups)
                {
                    //root->scope->group
                    var groupNode = scopeNode.GetOrCreateChild(Group, theGroup.Key, theGroup.Count());
                    foreach (var theClient in theGroup)
                    {
                        //root->scope->group->client
                        var clientNode = groupNode.GetOrCreateChild(Client, theClient.ClientId, 0);

                        var theConnections = signalRConnections.Where(x =>
                            x.ScopeId.MyEquals(theScope.Key) &&
                            x.ClientId.MyEquals(theClient.ClientId)).ToList();
                        clientNode.Value = theConnections.Count;

                        foreach (var theConnection in theConnections)
                        {
                            //root->scope->group->client->connection
                            clientNode.GetOrCreateChild(Connection, theConnection.ConnectionId, 0);
                        }
                    }
                }
            }

            //auto add empty signalr conn for UI
            foreach (var groupNode in rootNode.Children)
            {
                foreach (var clientNode in groupNode.Children)
                {
                    if (clientNode.Children.Count == 0)
                    {
                        clientNode.GetOrCreateChild(Connection, string.Empty, 0);
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

        public async Task UpdateMonitor(SignalREvent theEvent, IClientConnectionRepository connectionRepository, IScopeClientGroupRepository scopeClientGroupRepository, SignalRConnectionCache signalRConnectionCache)
        {
            if (Config.UpdateConnectionsEnabled)
            {
                var connections = connectionRepository.GetConnections(new GetConnectionsArgs());
                var scopeClientGroups = scopeClientGroupRepository.GetScopeClientGroups(new ScopeClientGroupLocate());
                
                var hubClients = theEvent.TryGetHubClients();
                var updateConnectionsArgs = UpdateConnectionsArgs.Create(connections);
                await UpdateConnections(hubClients, updateConnectionsArgs);

                var signalRConnections = signalRConnectionCache.Locates(new ClientConnectionLocate());
                var updateClientTreeArgs = UpdateClientTreeArgs.Create(scopeClientGroups, signalRConnections, connections);
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
