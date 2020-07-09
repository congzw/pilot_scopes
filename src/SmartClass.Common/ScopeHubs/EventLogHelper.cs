using System;
using System.Diagnostics;
using Newtonsoft.Json;
using SmartClass.Common.ScopeHubs.ClientMonitors;

namespace SmartClass.Common.ScopeHubs
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
        public HubClientLogHelper()
        {
            var monitorHelper = ManageMonitorHelper.Instance;
            //todo with a api
            monitorHelper.Config.UpdateMonitorInfoEnabled = true;
            monitorHelper.Config.IncludeConnections = true;
            monitorHelper.Config.ServerLogEnabled = true;
        }

        public void Log(object message, string prefix = "")
        {
            var theEvent = message as SignalREvent;
            if (theEvent == null)
            {
                Trace.WriteLine(string.Format("[SignalREvent]{0}=> {1}", prefix, message));
                return;
            }
            var hubClients = theEvent.TryGetHubClients();
            ManageMonitorHelper.Instance.ServerLog(hubClients, theEvent.SendArgs);
        }
    }
}
