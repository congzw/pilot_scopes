using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups
{
    public class AddToGroupEvent : BaseHubCrossEvent
    {
        public AddToGroup Args { get; set; }

        public AddToGroupEvent(Hub raiseHub, string scopeId, AddToGroup args) : base(raiseHub, scopeId)
        {
            Args = args;
        }

        public AddToGroupEvent(HubContextWrapper context, string scopeId, AddToGroup args) : base(context, scopeId)
        {
            Args = args;
        }
    }

    public class AddToGroup
    {
        public AddToGroup()
        {
            Items = new List<ScopeClientGroupLocate>();
        }
        public IList<ScopeClientGroupLocate> Items { get; set; }
    }
}