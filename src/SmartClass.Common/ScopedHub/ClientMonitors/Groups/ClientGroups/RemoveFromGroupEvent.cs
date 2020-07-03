using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.Groups.ClientGroups
{
    public class RemoveFromGroupEvent : BaseHubCrossEvent
    {
        public RemoveFromGroup Args { get; set; }

        public RemoveFromGroupEvent(Hub raiseHub, string scopeId, RemoveFromGroup args) : base(raiseHub, scopeId)
        {
            Args = args;
        }
    }

    public class RemoveFromGroup
    {
        public RemoveFromGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}
