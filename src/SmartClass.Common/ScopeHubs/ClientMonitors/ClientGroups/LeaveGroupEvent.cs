using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
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
}
