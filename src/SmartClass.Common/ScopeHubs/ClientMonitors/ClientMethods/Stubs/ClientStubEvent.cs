using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs
{
    public class ClientStubEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientStubEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub)
        {
            args.WithSendContext(SendContext);
            Args = args;
        }

        public ClientStubEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, ClientMethodArgs args) : base(hubContextWrapper, sendContext)
        {
            args.WithSendContext(SendContext);
            Args = args;
        }
    }
}
