using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Scopes
{
    public interface IScopeManager
    {
        Task ResetScope(ResetScopeEvent theEvent);
        Task UpdateScope(UpdateScopeEvent theEvent);
        Task<IList<ScopeContext>> GetScopeContexts();
    }

    #region events
    
    public class ResetScopeEvent : SignalREvent
    {
        public ResetScopeArgs Args { get; }

        public ResetScopeEvent(Hub raiseHub, ResetScopeArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public ResetScopeEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, ResetScopeArgs args) : base(hubContextWrapper, sendContext)
        {
            Args = args;
        }
    }

    public class UpdateScopeEvent : SignalREvent
    {
        public UpdateScopeArgs Args { get; set; }

        public UpdateScopeEvent(Hub raiseHub, UpdateScopeArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public UpdateScopeEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, UpdateScopeArgs args) : base(hubContextWrapper, sendContext)
        {
            Args = args;
        }
    }

    public class ResetScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }

    public class UpdateScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
    
    public class ResetScopeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ResetScopeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ResetScopeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (ResetScopeEvent)@event;
            await _clientMonitor.ResetScope(theEvent);
        }
    }

    public class UpdateScopeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public UpdateScopeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is UpdateScopeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (UpdateScopeEvent)@event;
            await _clientMonitor.UpdateScope(theEvent);
        }
    }

    #endregion
}
