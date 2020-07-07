using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods.Stubs
{
    public interface IOnCallClientStub
    {
        //客户端桩子方法的被动调用事件
        Task OnCallClientStub(OnCallClientStubEvent theEvent);
    }

    public class OnCallClientStubEvent : ScopedHubEvent
    {
        public ClientMethodArgs Args { get; set; }

        public OnCallClientStubEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }

        public OnCallClientStubEvent(HubContextWrapper context, ClientMethodArgs args) : base(context, args.ScopeId)
        {
            Args = args;
        }
    }


}
