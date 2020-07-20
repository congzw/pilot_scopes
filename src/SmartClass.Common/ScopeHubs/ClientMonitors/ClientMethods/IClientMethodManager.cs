using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        /// <summary>
        /// 客户端方法的调用事件
        /// </summary>
        /// <param name="theEvent"></param>
        /// <returns></returns>
        Task ClientMethod(ClientMethodEvent theEvent);
    }

    public class ClientMethodEvent : SignalREvent
    {
        public ClientMethodArgs Args { get; set; }

        public ClientMethodEvent(Hub raiseHub, ClientMethodArgs args) : base(raiseHub, args.SendContext)
        {
            Args = args;
        }

        public ClientMethodEvent(HubContextWrapper hubContextWrapper, ClientMethodArgs args) : base(hubContextWrapper, args.SendContext)
        {
            Args = args;
        }
    }

    public class ClientMethodEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ClientMethodEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ClientMethodEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            var theEvent = (ClientMethodEvent)@event;
            await _clientMonitor.ClientMethod(theEvent);
        }
    }
}