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
        public int Value { get; set; }

        public List<UpdateClientTreeArgs> Children { get; set; } = new List<UpdateClientTreeArgs>();

        public static UpdateClientTreeArgs Create(string type, string name, int value)
        {
            return new UpdateClientTreeArgs { Type = type, Name = name, Value = value };
        }

        public static UpdateClientTreeArgs Create(IList<ScopeClientGroup> list, IList<MyConnection> connections)
        {
            var scopes = list.GroupBy(x => x.ScopeId).ToList();
            var rootNode = Create("Root", "PV100", scopes.Count);
            
            foreach (var scope in scopes)
            {
                var groups = scope.GroupBy(x => x.Group).ToList();
                var scopeNode = Create("Classroom", scope.Key, groups.Count);
                rootNode.Children.Add(scopeNode);
                foreach (var @group in groups)
                {
                    var clients = @group.GroupBy(x => x.ClientId).ToList();
                    var groupNode = Create("Group", group.Key, clients.Count);
                    scopeNode.Children.Add(groupNode);
                    foreach (var client in clients)
                    {
                        var clientNode = Create("Client", client.Key, 1);
                        var scopeClientGroup = client.FirstOrDefault();
                        if (scopeClientGroup != null)
                        {
                            var theOne = connections.SingleOrDefault(x =>
                                x.GetScopeClientKey().MyEquals(scopeClientGroup.GetScopeClientKey()));
                            if (theOne != null)
                            {
                                var connNode = Create("Conn", theOne.ConnectionId, 0);
                                clientNode.Children.Add(connNode);
                            }
                        }
                        groupNode.Children.Add(clientNode);
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

        public Action<EventInvokeInfo> ProcessAction { get; set; }

        public Task EventInvoked(IHubClients<IClientProxy> hubClients, EventInvokeInfo info)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            if (Config.UpdateConnectionsEnabled)
            {
                //todo: add connections
                //info.Connections = ...
            }

            //info.Desc += scopeGroup;
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_EventInvoked, info);
        }

        public Task ServerLog(IHubClients<IClientProxy> hubClients, ServerLogInfo logInfo)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_ServerLog, logInfo);
        }

        public Task UpdateConnections(IHubClients<IClientProxy> hubClients, UpdateConnectionsArgs args)
        {
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            return hubClients.Groups(scopeGroup).SendAsync(HubConst.Monitor_MethodInClient_UpdateConnections, args);
        }

        public Task UpdateClientTree(IHubClients<IClientProxy> hubClients, UpdateClientTreeArgs args)
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
