using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class HubCallerContextCache
    {
        //signalR connectionId -> HubCallerContext
        public IDictionary<string, HubCallerContext> HubCallerContexts { get; set; } = new ConcurrentDictionary<string, HubCallerContext>(StringComparer.OrdinalIgnoreCase);


        public HubCallerContextCache AddCurrentConnectionCache(Hub hub)
        {
            this.HubCallerContexts[hub.Context.ConnectionId] = hub.Context;
            return this;
        }
        
        public HubCallerContext GetCurrentConnectionCache(Hub hub, string connectionId)
        {
            this.HubCallerContexts.TryGetValue(connectionId, out var value);
            return value;
        }

        public HubCallerContextCache RemoveCurrentConnectionCache(Hub hub)
        {
            this.HubCallerContexts[hub.Context.ConnectionId] = hub.Context;
            return this;
        }
    }
}