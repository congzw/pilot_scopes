using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public interface IClientConnectionRepository : IMyScoped
    {
        MyConnection GetConnection(IClientConnectionLocate locate);
        IList<MyConnection> GetConnections(IScopeKey locate);
        IEnumerable<MyConnection> Query();
        void AddOrUpdate(MyConnection connection);
        void Remove(MyConnection connection);
    }

    public class ClientConnectionRepository : IClientConnectionRepository
    {
        public MyConnection GetConnection(IClientConnectionLocate locate)
        {
            throw new System.NotImplementedException();
        }

        public IList<MyConnection> GetConnections(IScopeKey locate)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<MyConnection> Query()
        {
            throw new System.NotImplementedException();
        }

        public void AddOrUpdate(MyConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(MyConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}
