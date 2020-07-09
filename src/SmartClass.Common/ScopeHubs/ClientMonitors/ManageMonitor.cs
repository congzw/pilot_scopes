using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public class ClientStateInfo : IHaveBags
    {
        public ClientStateInfo()
        {
            Bags = BagsHelper.Create();
            Connections = new List<MyConnection>();
            InvokeAt = DateHelper.Instance.GetDateNow();
        }

        /// <summary>
        /// 当前触发的ScopeId
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// 当前触发的ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 交互分类的描述
        /// </summary>
        public string Desc { get; set; }

        public DateTime InvokeAt { get; set; }

        public IDictionary<string, object> Bags { get; set; }

        public IList<MyConnection> Connections { get; set; }
    }

    public class ManageMonitorHelper
    {
        public ManageMonitorConfig Config { get; set; } = new ManageMonitorConfig();

        public Action<ClientStateInfo> ProcessAction { get; set; }

        public Task UpdateMonitorInfo(IHubClients<IClientProxy> hubClients, IClientConnectionRepository repository, string invokeScopeId, string invokeClientId, string invokeDesc)
        {
            var monitorEnabled = Config.UpdateMonitorInfoEnabled;
            if (!monitorEnabled)
            {
                return Task.CompletedTask;
            }

            var monitorClientLocate = ClientConnectionLocate.Create()
                .WithScopeId(HubConst.Monitor_ScopeId)
                .WithClientId(HubConst.Monitor_ClientId);

            var manageClientConn = repository.GetConnection(monitorClientLocate);
            if (manageClientConn == null)
            {
                return Task.CompletedTask;
            }

            var monitorClientProxy = hubClients.Client(manageClientConn.ConnectionId);
            if (monitorClientProxy == null)
            {
                return Task.CompletedTask;
            }

            var invokeInfo = new ClientStateInfo();
            invokeInfo.ScopeId = invokeScopeId;
            invokeInfo.ClientId = invokeClientId;
            invokeInfo.Desc = invokeDesc;
            if (Config.IncludeConnections)
            {
                var theConnections = repository
                    .Query()
                    .Where(x => x.ScopeId.MyEquals(invokeScopeId))
                    .OrderBy(x => x.ScopeId)
                    .ThenBy(x => x.ClientId)
                    .ThenBy(x => x.CreateAt).ToList();
                invokeInfo.Connections = theConnections;
            }
            ProcessAction?.Invoke(invokeInfo);
            return monitorClientProxy.SendAsync(HubConst.Monitor_MethodInClient_UpdateMonitorInvokeInfo, invokeInfo);
        } 

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
        public bool IncludeConnections { get; set; }
    }
}
