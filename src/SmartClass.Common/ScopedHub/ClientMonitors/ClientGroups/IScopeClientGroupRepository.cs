using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups
{
    public interface IScopeClientGroupRepository
    {
        IEnumerable<ScopeClientGroup> QueryScopeClientGroups(IScopeClientGroupLocate args);
        ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args);
        void Add(ScopeClientGroup scopeClientGroup);
        void Remove(ScopeClientGroup scopeClientGroup);
    }
}