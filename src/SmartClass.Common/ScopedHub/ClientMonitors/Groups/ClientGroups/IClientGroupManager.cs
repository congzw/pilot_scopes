using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopedHub.ClientMonitors.Groups.ClientGroups
{
    public interface IClientGroupManager
    {
        Task AddToGroup(AddToGroup args);
        Task RemoveFromGroup(RemoveFromGroup args);
        Task<IList<ScopeClientGroup>> GetGroups(IScopeClientGroupLocate args);
    }

    public class ScopeClientGroup : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    public class GetScopeClientGroupsArgs : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}