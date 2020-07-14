using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
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
}