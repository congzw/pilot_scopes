using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class KickClientEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _manager;

        public KickClientEventHandler(IClientMonitor manager)
        {
            _manager = manager;
        }

        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is KickClientEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (KickClientEvent)@event;
            await _manager.KickClient(theEvent).ConfigureAwait(false);
        }
    }
}
