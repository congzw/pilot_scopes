using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public interface IClientGroupManager
    {
        Task JoinGroup(JoinGroupEvent @event);
        Task LeaveGroup(LeaveGroupEvent @event);
        Task<IList<ScopeClientGroup>> GetClientGroups(GetClientGroupsArgs args);
    }

    public class GetClientGroupsArgs : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    public class ScopeClientGroup : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}