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

    #endregion
}