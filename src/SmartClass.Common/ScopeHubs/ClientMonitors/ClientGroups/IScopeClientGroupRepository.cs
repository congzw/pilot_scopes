using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public interface IScopeClientGroupRepository
    {
        IEnumerable<ScopeClientGroupLocate> QueryScopeClientGroups(IScopeClientGroupLocate args);
        ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args);
        void Add(ScopeClientGroup scopeClientGroup);
        void Remove(ScopeClientGroup scopeClientGroup);
    }
}