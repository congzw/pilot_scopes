using System;
using System.Collections.Generic;
using System.Linq;
using SmartClass.Common.Data;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Domains;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public class ScopeClientGroupRepository : IScopeClientGroupRepository
    {
        private readonly IHblTempRepository _hblTempRepository;
        public ScopeClientGroupRepository(IHblTempRepository hblTempRepository)
        {
            _hblTempRepository = hblTempRepository;
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