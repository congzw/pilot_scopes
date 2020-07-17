using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public class SignalRConnectionCache
    {
        public IDictionary<string, SignalRConnection> Connections { get; set; } = new Dictionary<string, SignalRConnection>(StringComparer.OrdinalIgnoreCase);

        public void Set(SignalRConnection signalRConnection)
        {
            Connections[signalRConnection.ConnectionId] = signalRConnection;
        }

        public void Remove(string connectionId)
        {
            Connections.TryGetValue(connectionId, out var theOne);
            if (theOne != null)
            {
                Connections.Remove(connectionId);
            }
        }

        public SignalRConnection Get(string connectionId)
        {
            Connections.TryGetValue(connectionId, out var result);
            return result;
        }

        public IList<SignalRConnection> Locates(IClientConnectionLocate locate)
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));

            var query = Connections.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(locate.ScopeId))
            {
                query = query.Where(x => x.ScopeId.MyEquals(locate.ScopeId));
            }

            if (!string.IsNullOrWhiteSpace(locate.ClientId))
            {
                query = query.Where(x => x.ClientId.MyEquals(locate.ClientId));
            }

            if (!string.IsNullOrWhiteSpace(locate.ConnectionId))
            {
                query = query.Where(x => x.ConnectionId.MyEquals(locate.ConnectionId));
            }

            return query.ToList();
        }
    }

    public class SignalRConnection : IClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
    }
}
