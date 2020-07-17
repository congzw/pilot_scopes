using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;

namespace SmartClass.Common.ScopeHubs
{
    public class HubConst
    {
        #region Monitor

        public static string Monitor_ScopeId = "monitor";
        public static string Monitor_MethodInClient_EventInvoked = "eventInvoked";
        public static string Monitor_MethodInClient_ServerLog = "serverLog";
        public static string Monitor_MethodInClient_UpdateConnections = "updateConnections";
        public static string Monitor_MethodInClient_UpdateClientTree = "updateClientTree";
        public static string Monitor_MethodInHub_GetClientStates = "GetClientStates";

        #endregion

        public static string ScopeId_Default = "Default";
        public static string Args_ScopeId = "scopeId";
        public static string Args_ClientId = "clientId";
        public static string Args_ClientType = "clientType";
        public static string GroupName_All = string.Empty;

        public static string ClientMethod_StubKicked = "stubKicked";
        public static string ClientMethod_StubScopeUpdated = "stubScopeUpdated";
        public static string ClientMethod_StubNotify = "stubNotify";

        public static HubConst Ext = new HubConst();
    }

    public static class ClientMethodExtensions
    {
        //让前端必须大写
        private static string ClientMethod = "clientMethod";
        public static Task SendAsyncToClientMethod(this IClientProxy clientProxy, ClientMethodArgs args)
        {
            return clientProxy.SendAsync(ClientMethod, args);
        }
    }
}
