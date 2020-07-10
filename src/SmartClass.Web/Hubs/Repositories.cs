using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartClass.Common.Data;

// ReSharper disable once CheckNamespace
namespace SmartClass.Domains
{
    public interface IHblDbRepository : ISimpleRepository
    {
        //用于标识HBL持久化仓储
    }

    public interface IHblTempRepository : ISimpleRepository
    {
        //用于标识HBL临时仓储
    }

    public class HblRepositoryAdapter : IHblDbRepository, IHblTempRepository
    {
        private readonly ISimpleRepository _simpleRepository;

        public HblRepositoryAdapter(ISimpleRepository simpleRepository)
        {
            _simpleRepository = simpleRepository;
        }

        public IList<T> GetAll<T>() where T : class
        {
            return _simpleRepository.GetAll<T>();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _simpleRepository.Query<T>();
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _simpleRepository.Get<T>(predicate);
        }

        public T Get<T>(object id) where T : class
        {
            return _simpleRepository.Get<T>(id);
        }

        public void Add<T>(IEnumerable<T> entities) where T : class
        {
            _simpleRepository.Add<T>(entities);
        }

        public void Update<T>(IEnumerable<T> entities) where T : class
        {
            _simpleRepository.Update<T>(entities);
        }

        public void Delete<T>(IEnumerable<T> entities) where T : class
        {
            _simpleRepository.Delete<T>(entities);
        }

        public void Truncate<T>() where T : class
        {
            _simpleRepository.Truncate<T>();
        }

        public Task<List<T>> GetAllAsync<T>() where T : class
        {
            return _simpleRepository.GetAllAsync<T>();
        }

        public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _simpleRepository.GetAsync<T>(predicate);
        } 
    }
}
