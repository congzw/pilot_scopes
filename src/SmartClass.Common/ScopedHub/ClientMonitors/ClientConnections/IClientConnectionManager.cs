using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task OnKick(OnKickEvent theEvent);
    }

    #region events
    
    public class OnConnectedEvent : BaseHubEvent
    {
        public OnConnectedEvent(Hub raiseHub) : base(raiseHub, raiseHub.TryGetScopeId())
        {
        }
    }

    public class OnDisconnectedEvent : BaseHubEvent
    {
        public string Reason { get; set; }

        public OnDisconnectedEvent(Hub raiseHub, string reason) : base(raiseHub, raiseHub.TryGetScopeId())
        {
            Reason = reason;
        }
    }

    public class OnKickEvent : BaseHubCrossEvent
    {
        public KickClientArgs Args { get; }

        public OnKickEvent(Hub raiseHub, KickClientArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public OnKickEvent(HubContextWrapper context, KickClientArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class KickClientArgs : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public string Reason { get; set; }
    }

    #endregion
}