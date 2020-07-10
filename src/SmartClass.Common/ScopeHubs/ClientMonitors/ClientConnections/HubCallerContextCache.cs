using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class HubCallerContextCache
    {
        //signalR connectionId -> HubCallerContext
        internal IDictionary<string, HubCallerContext> HubCallerContexts { get; set; } = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);


        public HubCallerContextCache SetCache(Hub hub)
        {
            this.HubCallerContexts[hub.Context.ConnectionId] = hub.Context;
            return this;
        }

        public HubCallerContext GetCache(Hub hub, string connectionId)
        {
            this.HubCallerContexts.TryGetValue(connectionId, out var value);
            return value;
        }

        public HubCallerContextCache RemoveCache(Hub hub)
        {
            if (HubCallerContexts.ContainsKey(hub.Context.ConnectionId))
            {
                this.HubCallerContexts[hub.Context.ConnectionId] = hub.Context;
            }
            return this;
        }
    }
}