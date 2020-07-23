using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public interface IClientGroupManager
    {
        Task JoinGroup(JoinGroupEvent @event);
        Task LeaveGroup(LeaveGroupEvent @event);
        Task<IList<ScopeClientGroup>> GetClientGroups(GetClientGroupsArgs args);
    }

    public class JoinGroupEvent : SignalREvent
    {
        public JoinGroupArgs Args { get; set; }

        public JoinGroupEvent(Hub raiseHub, JoinGroupArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public JoinGroupEvent(HubContextWrapper hubContext, SendContext sendContext, JoinGroupArgs args) : base(hubContext, sendContext)
        {
            Args = args;
        }
    }

    public class JoinGroupArgs : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; } = new List<string>();
    }
    
    public class LeaveGroupEvent : SignalREvent
    {
        public LeaveGroupArgs Args { get; set; }

        public LeaveGroupEvent(Hub raiseHub, LeaveGroupArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public LeaveGroupEvent(HubContextWrapper hubContext, SendContext sendContext, LeaveGroupArgs args) : base(hubContext, sendContext)
        {
            Args = args;
        }
    }

    public class LeaveGroupArgs : IScopeGroupLocate
    {
        public LeaveGroupArgs()
        {
            ClientIds = new List<string>();
        }

        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }

    public class GetClientGroupsArgs : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    public class ScopeClientGroup : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    public class JoinGroupEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public JoinGroupEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; } = SignalREventHandlerOrder.System;

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

        public float HandleOrder { get; set; } = SignalREventHandlerOrder.System;

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