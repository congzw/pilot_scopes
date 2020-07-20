using System;
using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

namespace SmartClass.Common.ScopeHubs._Impl
{
    public class SignalREventBus
    {
        private readonly ISignalREventDispatcher _dispatcher;
        private readonly IClientConnectionRepository _connectionRepository;
        private readonly IScopeClientGroupRepository _scopeClientGroupRepository;
        private readonly SignalRConnectionCache _signalRConnectionCache;

        public SignalREventBus(ISignalREventDispatcher dispatcher, IClientConnectionRepository connectionRepository, IScopeClientGroupRepository scopeClientGroupRepository, SignalRConnectionCache signalRConnectionCache)
        {
            _dispatcher = dispatcher;
            _connectionRepository = connectionRepository;
            _scopeClientGroupRepository = scopeClientGroupRepository;
            _signalRConnectionCache = signalRConnectionCache;
        }

        public async Task Raise(ISignalREvent @event)
        {
            var manageMonitorHelper = ManageMonitorHelper.Instance;
            var theEvent = (SignalREvent)@event;
            try
            {
                await _dispatcher.Dispatch(@event);
                await manageMonitorHelper.TraceSignalREvent(theEvent, " handled").ConfigureAwait(false);
                await manageMonitorHelper.UpdateMonitor(theEvent, _connectionRepository, _scopeClientGroupRepository, _signalRConnectionCache);
            }
            catch (Exception ex)
            {
                await manageMonitorHelper.TraceSignalREvent(theEvent, " !ex").ConfigureAwait(false);
                await manageMonitorHelper.ServerLog(theEvent.TryGetHubClients(), new ServerLogInfo() { Category = theEvent.GetType().Name, Message = ex.Message });
            }
        }
    }
}
