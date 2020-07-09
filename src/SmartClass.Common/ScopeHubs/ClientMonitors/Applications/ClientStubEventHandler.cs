using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications
{
    public class ClientStubEventHandler : ISignalREventHandler
    {
        //private readonly IClientMonitor _clientMonitor;

        //public ClientStubEventHandler(IClientMonitor clientMonitor)
        //{
        //    _clientMonitor = clientMonitor;
        //}

        public float HandleOrder { get; set; }
        public bool ShouldHandle(ISignalREvent @event)
        {
            return @event is ClientStubEvent;
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            if (!ShouldHandle(@event))
            {
                return;
            }

            var theEvent = (ClientStubEvent)@event;
            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "ClientStub", JsonConvert.SerializeObject(theEvent.Args, Formatting.None)));
            var hubClients = theEvent.TryGetHubClients();
            await hubClients.All.SendAsync("ClientStub", theEvent.Args);
            //await _clientMonitor.ClientStub(theEvent);
        }
    }
}
