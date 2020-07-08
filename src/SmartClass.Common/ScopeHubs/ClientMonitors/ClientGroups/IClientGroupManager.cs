using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public interface IClientGroupManager
    {
        Task AddToGroup(AddToGroupArgs args);
        Task RemoveFromGroup(RemoveFromGroupArgs args);
        Task<IList<ScopeClientGroup>> GetGroups(IScopeKey args);
    }

    public class ScopeClientGroup : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
}