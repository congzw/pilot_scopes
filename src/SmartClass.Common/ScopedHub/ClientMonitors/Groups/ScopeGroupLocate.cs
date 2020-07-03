namespace SmartClass.Common.ScopedHub.ClientMonitors.Groups
{
    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }

        public static ScopeGroupLocate Create(string scopeId, string @group)
        {
            var theScope = string.IsNullOrWhiteSpace(scopeId) ? HubConst.DefaultScopeId : scopeId;
            var theGroup = string.IsNullOrWhiteSpace(group) ? HubConst.AllGroupId : group;
            return new ScopeGroupLocate {ScopeId = theScope, Group = theGroup };
        }
    }
}