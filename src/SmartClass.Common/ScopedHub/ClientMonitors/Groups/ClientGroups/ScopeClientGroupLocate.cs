using System;

namespace SmartClass.Common.ScopedHub.ClientMonitors.Groups.ClientGroups
{
    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
        
        public static ScopeClientGroupLocate Create(string scopeId, string @group, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            var theScope = string.IsNullOrWhiteSpace(scopeId) ? HubConst.DefaultScopeId : scopeId;
            var theGroup = string.IsNullOrWhiteSpace(group) ? HubConst.AllGroupId : group;
            return new ScopeClientGroupLocate { ScopeId = theScope, Group = theGroup, ClientId = clientId };
        }
    }
}
