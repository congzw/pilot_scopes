using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public interface IScopeClientGroupRepository : IMyScoped
    {
        IList<ScopeClientGroup> GetScopeClientGroups(IScopeClientGroupLocate args);
        ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args);
        void Add(ScopeClientGroup scopeClientGroup);
        void Remove(ScopeClientGroup scopeClientGroup);
    }
}