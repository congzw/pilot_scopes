using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes
{
    public class ClientInvokeEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientInvokeEvent(HubContextWrapper hubContext, ClientMethodArgs args) : base(hubContext, args.ScopeId)
        {
            Args = args;
        }

        public ClientInvokeEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }
    }
}