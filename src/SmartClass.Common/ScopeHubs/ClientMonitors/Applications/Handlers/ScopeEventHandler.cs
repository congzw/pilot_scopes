using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class ResetScopeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ResetScopeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ResetScopeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (ResetScopeEvent)@event;
            await _clientMonitor.ResetScope(theEvent);
        }
    }

    public class UpdateScopeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public UpdateScopeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is UpdateScopeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (UpdateScopeEvent)@event;
            await _clientMonitor.UpdateScope(theEvent);
        }
    }
}
