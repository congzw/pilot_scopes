using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public interface IClientConnectionRepository : IMyScoped
    {
        MyConnection GetConnection(IClientConnectionLocate locate);
        IList<MyConnection> GetConnections(GetConnectionsArgs args);
        IEnumerable<MyConnection> Query();
        void AddOrUpdate(MyConnection connection);
        void Remove(MyConnection connection);
    }

    public class GetConnectionsArgs : IScopeKey
    {
        public string ScopeId { get; set; }
        public bool? Online { get; set; }
    }
}
