using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartClass.Common.ScopeHubs.ClientMonitors;

// ReSharper disable once CheckNamespace
namespace SmartClass.Common.ScopeHubs
{
    public interface ISignalREventDispatcher
    {
        Task Dispatch(ISignalREvent hubEvent);
    }

    public class SignalREventDispatcher : ISignalREventDispatcher
    {
        public IEnumerable<ISignalREventHandler> Handlers { get; }

        public SignalREventDispatcher(IEnumerable<ISignalREventHandler> signalREventHandlers)
        {
            Handlers = signalREventHandlers ?? Enumerable.Empty<ISignalREventHandler>();
        }

        public async Task Dispatch(ISignalREvent hubEvent)
        {
            var sortedHandlers = Handlers
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            foreach (var handler in sortedHandlers)
            {
                //todo ex handing
                await handler.HandleAsync(hubEvent).ConfigureAwait(false);
            }
        }
    }

    public class SignalREventBus
    {
        private readonly ISignalREventDispatcher _dispatcher;

        public SignalREventBus(ISignalREventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Raise(ISignalREvent @event)
        {
            await TraceAsync(@event).ConfigureAwait(false);
            await _dispatcher.Dispatch(@event).ConfigureAwait(false);
        }

        private static Task TraceAsync(ISignalREvent @event)
        {
            var theEvent = (SignalREvent) @event;
            var hubClients = theEvent.TryGetHubClients();
            var eventName = theEvent.GetType().Name;

            Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", eventName,
                JsonConvert.SerializeObject(theEvent.Bags, Formatting.None)));
            var info = new MonitorInvokeInfo();
            info.ScopeId = theEvent.ScopeId;
            info.ClientId = ""; //todo: read from context claims
            info.Desc = eventName;
            //todo with a api
            ManageMonitorHelper.Instance.Config.UpdateMonitorInfoEnabled = true;
            return ManageMonitorHelper.Instance.UpdateMonitorInfo(hubClients, info);
        }
    }
}
