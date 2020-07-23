using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class HubCallerContextCache: IMySingleton
    {
        //signalR connectionId -> HubCallerContext
        internal IDictionary<string, HubCallerContext> HubCallerContexts { get; set; } = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);
        
        public HubCallerContextCache SetCache(string connectionId, HubCallerContext hubCallerContext)
        {
            this.HubCallerContexts[connectionId] = hubCallerContext;
            //Trace.WriteLine("[HubCallerContextCache] SetCache + " + connectionId);
            return this;
        }

        public HubCallerContext GetCache(string connectionId)
        {
            this.HubCallerContexts.TryGetValue(connectionId, out var value);
            return value;
        }
        
        public HubCallerContextCache RemoveCache(string connectionId)
        {
            if (HubCallerContexts.ContainsKey(connectionId))
            {
                this.HubCallerContexts.Remove(connectionId);
                //Trace.WriteLine("[HubCallerContextCache] RemoveCache - " + connectionId);
            }
            return this;
        }
    }
}