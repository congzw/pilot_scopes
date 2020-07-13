using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task KickClient(KickClientEvent theEvent);
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

    public class KickClientEvent : SignalREvent
    {
        public KickClientArgs Args { get; set; }

        public KickClientEvent(Hub raiseHub, KickClientArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public KickClientEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, KickClientArgs args) : base(hubContextWrapper, sendContext)
        {
            Args = args;
        }
    }

    public class KickClientArgs : IClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public string Reason { get; set; }
    }

    #endregion
}