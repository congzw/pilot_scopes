//using System.Diagnostics;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

//namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications.Handlers
//{
//    public class EventTraceHandler : ISignalREventHandler
//    {
//        public EventTraceHandler()
//        {
//            HandleOrder = -100;
//        }

//        public float HandleOrder { get; set; }

//        public bool ShouldHandle(ISignalREvent @event)
//        {
//            return @event is SignalREvent;
//        }

//        public async Task HandleAsync(ISignalREvent @event)
//        {
//            var theEvent = (SignalREvent)@event;
//            var hubClients = theEvent.TryGetHubClients();
//            var eventName = theEvent.GetType().Name;

//            EventLogHelper.Resolve().Log(string.Format("[_AnyHub] {0} >>>>>>>> {1}", eventName, JsonConvert.SerializeObject(theEvent.Bags, Formatting.None)));
//            var info = new MonitorInvokeInfo();
//            info.ScopeId = theEvent.ScopeId;
//            info.ClientId = ""; //todo: read from context claims
//            info.Desc = eventName;
//            //todo with a api
//            ManageMonitorHelper.Instance.Config.UpdateMonitorInfoEnabled = true;
//            await ManageMonitorHelper.Instance.UpdateMonitorInfo(hubClients, info);
//        }
//    }
//}
