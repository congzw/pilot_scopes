using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs
{
    public class ClientStubEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientStubEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.SendContext)
        {
            Args = args;
        }

        public ClientStubEvent(HubContextWrapper hubContextWrapper, ClientMethodArgs args) : base(hubContextWrapper, args.SendContext)
        {
            Args = args;
        }
    }
}
