using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs
{
    public class ClientStubEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientStubEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public ClientStubEvent(HubContextWrapper context, ClientMethodArgs args) : base(context, new SendArgs())
        {
            Args = args;
        }
    }


}
