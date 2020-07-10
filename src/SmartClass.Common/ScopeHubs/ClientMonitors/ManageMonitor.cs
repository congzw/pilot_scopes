using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public class EventInvokeInfo : ISendArgs
    {
        public string Desc { get; set; }
        public string ConnectionId { get; set; }
        public SendArgs SendArgs { get; set; }
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

    public class ServerLogInfo
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public object Details { get; set; }

        public static ServerLogInfo Create(string message, object details = null, string category = "")
        {
            return new ServerLogInfo(){Category = "[ServerLog]" + category, Message = message, Details = details};
        }
    }

    public class ManageMonitorHelper
    {
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



        //public Task UpdateMonitorInfo(IHubClients<IClientProxy> hubClients, IClientConnectionRepository repository, string invokeScopeId, string invokeClientId, string invokeDesc)
        //{
        //    var monitorEnabled = Config.UpdateMonitorInfoEnabled;
        //    if (!monitorEnabled)
        //    {
        //        return Task.CompletedTask;
        //    }

        //    var monitorClientLocate = ClientConnectionLocate.Create()
        //        .WithScopeId(HubConst.Monitor_ScopeId)
        //        .WithClientId(HubConst.Monitor_ClientId);

        //    var manageClientConn = repository.GetConnection(monitorClientLocate);
        //    if (manageClientConn == null)
        //    {
        //        return Task.CompletedTask;
        //    }

        //    var monitorClientProxy = hubClients.Client(manageClientConn.ConnectionId);
        //    if (monitorClientProxy == null)
        //    {
        //        return Task.CompletedTask;
        //    }

        //    var invokeInfo = new MonitorInvokeInfo();
        //    invokeInfo.ScopeId = invokeScopeId;
        //    invokeInfo.ClientId = invokeClientId;
        //    invokeInfo.Desc = invokeDesc;
        //    if (Config.IncludeConnections)
        //    {
        //        var theConnections = repository
        //            .Query()
        //            .Where(x => x.ScopeId.MyEquals(invokeScopeId))
        //            .OrderBy(x => x.ScopeId)
        //            .ThenBy(x => x.ClientId)
        //            .ThenBy(x => x.CreateAt).ToList();
        //        invokeInfo.Connections = theConnections;
        //    }
        //    ProcessAction?.Invoke(invokeInfo);
        //    return monitorClientProxy.SendAsync(HubConst.Monitor_MethodInClient_UpdateMonitorInvokeInfo, invokeInfo);
        //} 

        public static ManageMonitorHelper Instance = new ManageMonitorHelper();
    }

    public class ManageMonitorConfig
    {
        /// <summary>
        /// 指示是否由服务器端主动推送连接状态信息到Monitor
        /// 客户端可以选择主动接受也可以被动接受
        /// </summary>
        public bool UpdateMonitorInfoEnabled { get; set; }

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
