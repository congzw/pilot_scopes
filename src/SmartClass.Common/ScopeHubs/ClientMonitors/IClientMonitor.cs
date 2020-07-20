using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public interface IClientMonitor : IMyScoped,
        IClientConnectionManager,
        IClientMethodManager,
        IClientGroupManager,
        IScopeManager
    {
    }
}
