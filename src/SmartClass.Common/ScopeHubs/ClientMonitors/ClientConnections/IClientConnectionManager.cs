using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
    }

    #region events
    
    public class OnConnectedEvent : SignalREvent
    {
        public OnConnectedEvent(Hub raiseHub) : base(raiseHub)
        {
        }
    }

    public class OnDisconnectedEvent : SignalREvent
    {
        public string Reason { get; set; }

        public OnDisconnectedEvent(Hub raiseHub, string reason) : base(raiseHub)
        {
            Reason = reason;
        }
    }
    
    public class OnConnectedEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public OnConnectedEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; } = SignalREventHandlerOrder.System;
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

    public class OnDisconnectedEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _manager;

        public OnDisconnectedEventHandler(IClientMonitor manager)
        {
            _manager = manager;
        }

        public float HandleOrder { get; set; } = SignalREventHandlerOrder.System;

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

    #endregion
}