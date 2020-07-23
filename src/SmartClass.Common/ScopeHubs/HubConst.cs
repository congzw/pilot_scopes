﻿namespace SmartClass.Common.ScopeHubs
{
    public class HubConst
    {
        #region Monitor

        public static string Monitor_ScopeId = "monitor";
        public static string Monitor_MethodInClient_EventInvoked = "eventInvoked";
        public static string Monitor_MethodInClient_ServerLog = "serverLog";
        public static string Monitor_MethodInClient_UpdateConnections = "updateConnections";
        public static string Monitor_MethodInClient_UpdateClientTree = "updateClientTree";

        #endregion

        public static string Args_ScopeId = "scopeId";
        public static string Args_ClientId = "clientId";
        public static string Args_UserId = "userId";
        public static string Args_ClientType = "clientType";
        public static string GroupName_All = string.Empty;

        internal static string OnClientMethod = "onClientMethod";
        public static string ClientMethod_StubKicked = "stubKicked";
        public static string ClientMethod_StubScopeUpdated = "stubScopeUpdated";
        public static string ClientMethod_StubNotify = "stubNotify";

        public static HubConst Ext = new HubConst();
    }
}
