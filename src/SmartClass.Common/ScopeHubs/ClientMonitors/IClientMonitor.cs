using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public interface IClientMonitor : 
        IClientConnectionManager,
        IClientMethodManager,
        IScopeGroupManager,
        IClientGroupManager,
        IScopeManager
    {
    }
}
