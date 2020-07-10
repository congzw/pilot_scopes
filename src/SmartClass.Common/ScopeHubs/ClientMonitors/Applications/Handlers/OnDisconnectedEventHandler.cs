using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class OnDisconnectedEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _manager;

        public OnDisconnectedEventHandler(IClientMonitor manager)
        {
            _manager = manager;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent hubEvent)
        {
            return hubEvent is OnDisconnectedEvent;
        }

        public Task HandleAsync(ISignalREvent hubEvent)
        {
            var theEvent = (OnDisconnectedEvent)hubEvent;
            return _manager.OnDisconnected(theEvent);
        }
    }
}