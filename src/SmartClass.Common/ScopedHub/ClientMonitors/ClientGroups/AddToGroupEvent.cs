using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups
{
    public class AddToGroupEvent : ScopedHubEvent
    {
        public AddToGroup Args { get; set; }

        public AddToGroupEvent(Hub raiseHub, AddToGroup args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public AddToGroupEvent(HubContextWrapper context, AddToGroup args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }

    public class AddToGroup : IScopeGroupLocate
    {
        public AddToGroup()
        {
            ClientIds = new List<string>();
        }
        public string ScopeId { get; set; }
        public string Group { get; set; }
        public IList<string> ClientIds { get; set; }
    }
}