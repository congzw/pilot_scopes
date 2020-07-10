using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class ClientInvokeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ClientInvokeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ClientInvokeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            if (!ShouldHandle(@event))
            {
                return;
            }
            //todo: ClientInvoke process bus
            var theEvent = (ClientInvokeEvent)@event;
            await _clientMonitor.ClientInvoke(theEvent);
        }
    }
}