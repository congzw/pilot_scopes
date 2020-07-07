using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups
{
    public class RemoveFromGroupEvent : ScopedHubEvent
    {
        public RemoveFromGroup Args { get; set; }

        public RemoveFromGroupEvent(Hub raiseHub, RemoveFromGroup args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }
    }

    public class RemoveFromGroup : IScopeGroupLocate
    {
        public RemoveFromGroup()
        {
            ClientIds = new List<string>();
        }

        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }
}
