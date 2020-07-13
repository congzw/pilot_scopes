using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class HubCallerContextCache
    {
        //signalR connectionId -> HubCallerContext
        internal IDictionary<string, HubCallerContext> HubCallerContexts { get; set; } = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);


        public HubCallerContextCache SetCache(string connectionId, HubCallerContext hubCallerContext)
        {
            this.HubCallerContexts[connectionId] = hubCallerContext;
            //Trace.WriteLine("[HubCallerContextCache] SetCache + " + connectionId + " = " + string.Join(',', this.HubCallerContexts.Keys.ToList()));
            Trace.WriteLine("[HubCallerContextCache] SetCache + " + connectionId);
            return this;
        }

        public HubCallerContext GetCache(string connectionId)
        {
            this.HubCallerContexts.TryGetValue(connectionId, out var value);
            return value;
        }

        //public HubCallerContextCache RemoveCache(HubCallerContext hubCallerContext)
        //{
        //    if (hubCallerContext == null) throw new ArgumentNullException(nameof(hubCallerContext));
        //    var connectionId = hubCallerContext.ConnectionId;
        //    if (HubCallerContexts.ContainsKey(connectionId))
        //    {
        //        //Trace.WriteLine("[HubCallerContextCache] RemoveCache => " + connectionId);
        //        this.HubCallerContexts.Remove(connectionId);
        //        //Trace.WriteLine("[HubCallerContextCache] RemoveCache - " + connectionId + " = " + string.Join(',', this.HubCallerContexts.Keys.ToList()));
        //        Trace.WriteLine("[HubCallerContextCache] RemoveCache - " + connectionId);
        //    }
        //    return this;
        //}

        public HubCallerContextCache RemoveCache(string connectionId)
        {
            if (HubCallerContexts.ContainsKey(connectionId))
            {
                this.HubCallerContexts.Remove(connectionId);
                Trace.WriteLine("[HubCallerContextCache] RemoveCache - " + connectionId);
            }
            return this;
        }
    }
}