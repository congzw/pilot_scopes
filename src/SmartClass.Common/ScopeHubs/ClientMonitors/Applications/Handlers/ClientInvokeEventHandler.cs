using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class ClientInvokeEventHandler : ISignalREventHandler
    {
        private readonly IClientMonitor _clientMonitor;

        public ClientInvokeEventHandler(IClientMonitor clientMonitor)
        {
            _clientMonitor = clientMonitor;
        }

        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ClientInvokeEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            if (!ShouldHandle(@event))
            {
                return;
            }
            //todo: ClientInvoke process bus
            var theEvent = (ClientInvokeEvent)@event;
            //Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "ClientInvoke", JsonConvert.SerializeObject(theEvent.Args, Formatting.None)));
            //var hubClients = theEvent.TryGetHubClients();
            //await hubClients.All.SendAsync("ClientInvoke", theEvent.Args);

            await _clientMonitor.ClientInvoke(theEvent);
        }
    }
}