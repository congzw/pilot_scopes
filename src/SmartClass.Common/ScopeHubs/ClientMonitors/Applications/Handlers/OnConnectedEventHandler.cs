using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class OnConnectedEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public OnConnectedEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is OnConnectedEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (OnConnectedEvent)@event;
            await _clientMonitor.OnConnected(theEvent).ConfigureAwait(false);
        }
    }
}