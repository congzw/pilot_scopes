using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;
using SmartClass.Common.ScopedHub.Scopes;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task OnKick(KickClientEvent theEvent);
        Task ScopeReset(ScopeResetEvent theEvent);
        Task ScopeUpdate(ScopeUpdateEvent theEvent);
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

    public class KickClientEvent : BaseHubCrossEvent
    {
        public KickClientArgs Args { get; }

        public KickClientEvent(Hub raiseHub, KickClientArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public KickClientEvent(HubContextWrapper context, KickClientArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class ScopeResetEvent : BaseHubCrossEvent
    {
        public ScopeContext Args { get; }

        public ScopeResetEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ScopeResetEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class ScopeUpdateEvent : BaseHubCrossEvent
    {
        public ScopeContext Args { get; set; }

        public ScopeUpdateEvent(Hub raiseHub, ScopeContext args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ScopeUpdateEvent(HubContextWrapper context, ScopeContext args) : base(context, args.ScopeId)
        {
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