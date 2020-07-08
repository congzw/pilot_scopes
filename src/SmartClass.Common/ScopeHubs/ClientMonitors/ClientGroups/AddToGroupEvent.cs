using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public class AddToGroupEvent : SignalREvent
    {
        public AddToGroupArgs Args { get; set; }

        public AddToGroupEvent(Hub raiseHub, AddToGroupArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public AddToGroupEvent(HubContextWrapper context, AddToGroupArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class AddToGroupArgs : IScopeGroupLocate
    {
        public AddToGroupArgs()
        {
            ClientIds = new List<string>();
        }
        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }
}