using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.Applications;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections
{
    public interface IClientConnectionManager
    {
        Task OnConnected(OnConnectedEvent theEvent);
        Task OnDisconnected(OnDisconnectedEvent theEvent);
        Task KickClient(KickClientEvent theEvent);
        Task ResetScope(ResetScopeEvent theEvent);
        Task UpdateScope(UpdateScopeEvent theEvent);
    }

    #region events
    
    public class OnConnectedEvent : ScopedHubEvent
    {
        public OnConnectedEvent(Hub raiseHub) : base(raiseHub, raiseHub.TryGetScopeId())
        {
        }
    }

    public class OnDisconnectedEvent : ScopedHubEvent
    {
        public string Reason { get; set; }

        public OnDisconnectedEvent(Hub raiseHub, string reason) : base(raiseHub, raiseHub.TryGetScopeId())
        {
            Reason = reason;
        }
    }

    public class KickClientEvent : ScopedHubEvent
    {
        public KickClientArgs Args { get; set; }

        public KickClientEvent(Hub raiseHub, KickClientArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public KickClientEvent(HubContextWrapper context, KickClientArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class ResetScopeEvent : ScopedHubEvent
    {
        public ResetScopeArgs Args { get; }

        public ResetScopeEvent(Hub raiseHub, ResetScopeArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public ResetScopeEvent(HubContextWrapper context, ResetScopeArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class UpdateScopeEvent : ScopedHubEvent
    {
        public UpdateScopeArgs Args { get; set; }

        public UpdateScopeEvent(Hub raiseHub, UpdateScopeArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public UpdateScopeEvent(HubContextWrapper context, UpdateScopeArgs args) : base(context, args.ScopeId)
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