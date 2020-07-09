using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
{
    public class ClientStubEventHandler : ISignalREventHandler
    {
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
            EventLogHelper.Resolve().Log(theEvent);
            var hubClients = theEvent.TryGetHubClients();
            await hubClients.All.SendAsync("ClientStub", theEvent.Args);
            //await _clientMonitor.ClientStub(theEvent);
        }
    }
}
