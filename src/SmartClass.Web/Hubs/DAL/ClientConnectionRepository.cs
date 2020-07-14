using System;
using System.Collections.Generic;
using System.Linq;
using SmartClass.Common.Data;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Domains;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public class ClientConnectionRepository : IClientConnectionRepository, IScopeClientGroupRepository
    {
        private readonly IHblTempRepository _hblTempRepository;
        public ClientConnectionRepository(IHblTempRepository hblTempRepository)
        {
            _hblTempRepository = hblTempRepository;
        }

        public MyConnection GetConnection(IClientConnectionLocate locate)
        {
            var query = Query();
            if (locate == null)
            {
                return null;
            }

            var connection = query.Locate(locate);
            return connection;
        }

        public IList<MyConnection> GetConnections(GetConnectionsArgs args)
        {
            var query = Query();
            if (args.ScopeId != null)
            {
                query = query.Where(x => x.ScopeId == args.ScopeId);
            }
            if (args.Online != null)
            {
                query = query.Where(x => x.ConnectionId != string.Empty);
            }
            return query.ToList();
        }

        public IList<MyConnection> GetConnections(IEnumerable<IClientConnectionLocate> locates)
        {
            var connections = new List<MyConnection>();
            foreach (var locate in locates)
            {
                var connection = GetConnection(locate);
                if (connection != null)
                {
                    connections.Add(connection);
                }
            }

            return connections;
        }

        public IEnumerable<MyConnection> Query()
        {
            var query = _hblTempRepository.Query<MyConnection>();
            return query;
        }

        public void AddOrUpdate(MyConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            var theOne = GetConnection(connection);
            if (theOne == null)
            {
                _hblTempRepository.Add<MyConnection>(connection);
            }
            else
            {
                if (connection != theOne)
                {
                    connection.CopyTo(theOne);
                }
                _hblTempRepository.Update<MyConnection>(theOne);
            }
        }

        public void Remove(MyConnection connection)
        {
            var theOne = GetConnection(connection);
            if (theOne != null)
            {
                _hblTempRepository.Delete<MyConnection>(theOne);
            }
        }

        public IList<ScopeClientGroup> GetScopeClientGroups(IScopeClientGroupLocate args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var query = _hblTempRepository.Query<ScopeClientGroup>();
            if (!string.IsNullOrWhiteSpace(args.ScopeId))
            {
                query = query.Where(x => x.ScopeId == args.ScopeId);
            }

            if (!string.IsNullOrWhiteSpace(args.Group))
            {
                query = query.Where(x => x.Group == args.Group);
            }
            
            if (!string.IsNullOrWhiteSpace(args.ClientId))
            {
                query = query.Where(x => x.ClientId == args.ClientId);
            }

            var list = query.ToList();
            return list;
        }


        public ScopeClientGroup GetScopeClientGroup(IScopeClientGroupLocate args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var query = _hblTempRepository.Query<ScopeClientGroup>();
            return query.SingleOrDefault(x =>
                x.ScopeId == args.ScopeId && x.Group == args.Group && x.ClientId == args.ClientId);
        }

        public void Add(ScopeClientGroup args)
        {
            var allLocateKeysHasValue = args.AllLocateKeysHasValue(out var msg);
            if (!allLocateKeysHasValue)
            {
                throw new ArgumentException(msg);
            }
            var query = _hblTempRepository.Query<ScopeClientGroup>();
            var theOne = query.SingleOrDefault(x => x.ScopeId == args.ScopeId && x.Group == args.Group && x.ClientId == args.ClientId);
            if (theOne != null)
            {
                return;
            }
            _hblTempRepository.Add(args);
        }

        public void Remove(ScopeClientGroup args)
        {
            var allLocateKeysHasValue = args.AllLocateKeysHasValue(out var msg);
            if (!allLocateKeysHasValue)
            {
                throw new ArgumentException(msg);
            }

            var query = _hblTempRepository.Query<ScopeClientGroup>();
            var theOne = query.SingleOrDefault(x => x.ScopeId == args.ScopeId && x.Group == args.Group && x.ClientId == args.ClientId);
            if (theOne == null)
            {
                return;
            }
            _hblTempRepository.Delete(theOne);
        }
    }
}
