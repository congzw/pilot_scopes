namespace SmartClass.Common.ScopeHubs
{
    public class HubConst
    {
        #region Monitor

        public static string Monitor_ScopeId = "monitor_scope";
        public static string Monitor_ClientId = "monitor_client";
        public static string Monitor_MethodInClient_UpdateConnections = "updateConnections";
        public static string Monitor_MethodInClient_EventInvoked = "eventInvoked";
        public static string Monitor_MethodInClient_ServerLog = "serverLog";
        public static string Monitor_MethodInHub_GetClientStates = "GetClientStates";

        #endregion

        public static string ScopeId_Default = "Default";
        public static string Args_ScopeId = "scopeId";
        public static string Args_ClientId = "clientId";
        public static string GroupName_All = string.Empty;

        public static string ClientStub = "ClientStub";
        public static string ClientInvoke = "ClientInvoke";
        public static string ClientMethod_Notify = "notify";

        public static HubConst Ext = new HubConst();
    }
}
