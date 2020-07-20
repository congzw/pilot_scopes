using System;
using System.Collections.Generic;
using System.Linq;
using SmartClass.Common.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Scopes
{
    public static class ScopeContextExtensions
    {
        private static string Connections = "Connections";

        public static ScopeContext OnConnected(this ScopeContext scopeContext, ClientConnectionLocate locate)
        {
            var clientConnectionLocates = scopeContext.GetBagValue(Connections, new List<ClientConnectionLocate>());
            var theOne = clientConnectionLocates.FirstOrDefault(x => x.SameLocateValue(locate));
            if (theOne == null)
            {
                clientConnectionLocates.Add(locate);
            }
            scopeContext.SetBagValue(Connections, clientConnectionLocates);
            return scopeContext;
        }

        public static ScopeContext OnDisconnected(this ScopeContext scopeContext, ClientConnectionLocate locate)
        {
            var clientConnectionLocates = scopeContext.GetBagValue(Connections, new List<ClientConnectionLocate>());
            var theOne = clientConnectionLocates.FirstOrDefault(x => x.SameLocateValue(locate));
            if (theOne != null)
            {
                clientConnectionLocates.Remove(theOne);
            }
            scopeContext.SetBagValue(Connections, clientConnectionLocates);
            return scopeContext;
        }
    }
}
