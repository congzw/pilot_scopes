using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class JoinGroupEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public JoinGroupEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is JoinGroupEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (JoinGroupEvent)@event;
            await _clientMonitor.JoinGroup(theEvent);
        }
    }

    public class LeaveGroupEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public LeaveGroupEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is LeaveGroupEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (LeaveGroupEvent)@event;
            await _clientMonitor.LeaveGroup(theEvent);
        }
    }
}
