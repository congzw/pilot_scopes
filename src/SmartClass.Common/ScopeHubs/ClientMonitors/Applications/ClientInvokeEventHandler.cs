using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications
{
    public class ClientInvokeEventHandler : ISignalREventHandler
    {
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
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "ClientInvoke", JsonConvert.SerializeObject(theEvent.Args, Formatting.None)));
            var hubClients = theEvent.TryGetHubClients();
            await hubClients.All.SendAsync("ClientInvoke", theEvent.Args);
        }
    }
}