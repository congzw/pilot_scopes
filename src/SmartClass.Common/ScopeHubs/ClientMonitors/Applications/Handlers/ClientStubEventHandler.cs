using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class ClientStubEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ClientStubEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ClientStubEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (ClientStubEvent)@event;
            await _clientMonitor.ClientStub(theEvent).ConfigureAwait(false);
        }
    }
}
