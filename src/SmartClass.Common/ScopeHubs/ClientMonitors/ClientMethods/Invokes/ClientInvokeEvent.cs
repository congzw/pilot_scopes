using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes
{
    public class ClientInvokeEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientInvokeEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, raiseHub.GetSendFrom().GetSendContext())
        {
            Args = args;
        }

        public ClientInvokeEvent(HubContextWrapper hubContextWrapper, SendContext sendContext, ClientMethodArgs args) : base(hubContextWrapper, sendContext)
        {
            Args = args;
        }
    }
}