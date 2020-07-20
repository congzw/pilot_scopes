using System;
using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class ScopeClientConnectionKeyMaps : IMySingleton
    {
        public ScopeClientConnectionKeyMaps SetCache(IClientConnectionLocate connectionLocate)
        {
            if (connectionLocate == null) throw new ArgumentNullException(nameof(connectionLocate));

            var item = ClientConnectionLocate.CreateFrom(connectionLocate);

            var scopeClientKey = connectionLocate.GetScopeClientKey();
            ScopeClientKeyMap[scopeClientKey] = item;

            var connectionId = connectionLocate.ConnectionId;
            ConnectionKeyMap[connectionId] = item;

            return this;
        }
        
        public ClientConnectionLocate TryGetByScopeClientKey(string scopeId, string clientId)
        {
            var scopeClientLocate = new ScopeClientLocate();
            scopeClientLocate.WithScopeId(scopeId);
            scopeClientLocate.WithClientId(clientId);
            ScopeClientKeyMap.TryGetValue(scopeClientLocate.GetScopeClientKey(), out var result);
            return result;
        }

        public ClientConnectionLocate TryConnectionKeyKey(string connectionId)
        {
            ConnectionKeyMap.TryGetValue(connectionId, out var result);
            return result;
        }

        public ScopeClientConnectionKeyMaps RemoveCache(IClientConnectionLocate connectionLocate)
        {
            if (connectionLocate == null) throw new ArgumentNullException(nameof(connectionLocate));

            var scopeClientKey = connectionLocate.GetScopeClientKey();
            if (ScopeClientKeyMap.ContainsKey(scopeClientKey))
            {
                ScopeClientKeyMap.Remove(scopeClientKey);
            }

            var connectionId = connectionLocate.ConnectionId;
            if (ConnectionKeyMap.ContainsKey(connectionId))
            {
                ConnectionKeyMap.Remove(connectionId);
            }
            return this;
        }

        public ScopeClientConnectionKeyMaps Clear(IClientConnectionLocate connectionLocate)
        {
            ConnectionKeyMap.Clear();
            ScopeClientKeyMap.Clear();
            return this;
        }
        
        internal IDictionary<string, ClientConnectionLocate> ConnectionKeyMap { get; set; } = new Dictionary<string, ClientConnectionLocate>(StringComparer.OrdinalIgnoreCase);
        internal IDictionary<string, ClientConnectionLocate> ScopeClientKeyMap { get; set; } = new Dictionary<string, ClientConnectionLocate>(StringComparer.OrdinalIgnoreCase);
    }
}