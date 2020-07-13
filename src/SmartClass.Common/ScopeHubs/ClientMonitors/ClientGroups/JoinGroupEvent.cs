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

        public JoinGroupEvent(HubContextWrapper context, JoinGroupArgs args) : base(context, new SendArgs())
        {
            Args = args;
        }
    }

    public class JoinGroupArgs : IScopeGroupLocate
    {
        public JoinGroupArgs()
        {
            ClientIds = new List<string>();
        }
        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }
}