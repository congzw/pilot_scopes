using System;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes
{
    public class ClientInvokeEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientInvokeEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.SendContext)
        {
            Args = args;
        }

        public ClientInvokeEvent(HubContextWrapper hubContextWrapper, ClientMethodArgs args) : base(hubContextWrapper, args.SendContext)
        {
            Args = args;
        }
    }
}