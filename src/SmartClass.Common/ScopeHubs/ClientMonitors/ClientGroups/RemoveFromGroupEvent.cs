using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    public class RemoveFromGroupEvent : SignalREvent
    {
        public RemoveFromGroupArgs Args { get; set; }

        public RemoveFromGroupEvent(Hub raiseHub, RemoveFromGroupArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }
    }

    public class RemoveFromGroupArgs : IScopeGroupLocate
    {
        public RemoveFromGroupArgs()
        {
            ClientIds = new List<string>();
        }

        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }
}
