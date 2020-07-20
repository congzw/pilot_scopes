using System;
using System.Diagnostics;

namespace SmartClass.Common.ScopeHubs.ClientMonitors
{
    public interface IEventLogHelper
    {
        void Log(object message, string prefix = "");
    }

    public class EventLogHelper : IEventLogHelper
    {
        public void Log(object message, string prefix = "")
        {
            Trace.WriteLine(string.Format("[SignalREvent]{0}=> {1}", prefix, message));
        }

        #region for extensions

        private static readonly Lazy<IEventLogHelper> _lazy = new Lazy<IEventLogHelper>(() => new EventLogHelper());
        public static Func<IEventLogHelper> Resolve = () => _lazy.Value;

        #endregion
    }

    public class HubClientLogHelper : IEventLogHelper
    {
        public void Log(object message, string prefix = "")
        {
            //todo: use simple logger
            var theEvent = message as SignalREvent;
            if (theEvent == null)
            {
                Trace.WriteLine(string.Format("[SignalREvent]{0}=> {1}", prefix, message));
                return;
            }
            var hubClients = theEvent.TryGetHubClients();
            
            ManageMonitorHelper.Instance.ServerLog(hubClients, ServerLogInfo.Create(theEvent.GetType().Name, theEvent.SendContext));
        }
    }
}
