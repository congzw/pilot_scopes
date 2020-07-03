using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods.Invokes
{
    public interface IOnClientInvoke
    {
        //客户端方法的主动调用事件
        Task OnClientInvoke(OnClientInvokeEvent theEvent);
    }
    
    public class OnClientInvokeEvent : BaseHubCrossEvent
    {
        public ClientMethodArgs Args { get; set; }

        public OnClientInvokeEvent(HubContextWrapper hubContext, ClientMethodArgs args) : base(hubContext, args.ScopeId)
        {
            Args = args;
        }

        public OnClientInvokeEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.ScopeId)
        {
            Args = args;
        }
    }
}