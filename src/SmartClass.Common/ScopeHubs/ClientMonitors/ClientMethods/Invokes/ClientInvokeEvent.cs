using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes
{
    public class ClientInvokeEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientInvokeEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, raiseHub.GetSendFrom().GetSendContext())
        {
            args.WithSendContext(SendContext);
            Args = args;
        }

        public ClientInvokeEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, ClientMethodArgs args) : base(hubContextWrapper, sendContext)
        {
            args.WithSendContext(SendContext);
            Args = args;
        }
    }
}