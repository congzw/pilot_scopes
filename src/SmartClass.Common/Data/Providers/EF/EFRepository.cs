using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartClass.Common.Data.Providers.EF
{
    public class EFRepository : ISimpleRepository
    {
        private readonly DbContext _dbContext;

        public EFRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<T> GetAll<T>() where T : class
        {
            return Query<T>().ToList();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var theOne = _dbContext.Set<T>().SingleOrDefault(predicate);
            return theOne;
        }

        public T Get<T>(object id) where T : class
        {
            //how to solve eager loading?
            return _dbContext.Set<T>().Find(id);
        }

        public void Add<T>(IEnumerable<T> entities) where T : class
        {
            var dbSet = _dbContext.Set<T>();
            var array = entities.ToArray();
            if (array.Length == 1)
            {
                dbSet.Add(array[0]);
            }
            else
            {
                dbSet.AddRange(array);
            }
            //todo: abstract save job to uint of work
            _dbContext.SaveChanges();
        }

        public void Update<T>(IEnumerable<T> entities) where T : class
        {
            var dbSet = _dbContext.Set<T>();
            var array = entities.ToArray();
            if (array.Length == 1)
            {
                dbSet.Update(array[0]);
            }
            else
            {
                dbSet.UpdateRange(array);
            }
            //todo: abstract save job to uint of work
            _dbContext.SaveChanges();
        }

        public void Delete<T>(IEnumerable<T> entities) where T : class
        {
            var dbSet = _dbContext.Set<T>();
            var array = entities.ToArray();
            if (array.Length == 1)
            {
                dbSet.Remove(array[0]);
            }
            else
            {
                dbSet.RemoveRange(array);
            }
            //todo: abstract save job to uint of work
            _dbContext.SaveChanges();
        }

        public void Truncate<T>() where T : class
        {
            //todo:how to truncate?
            var dbSet = _dbContext.Set<T>();
            var list = dbSet.ToList();
            dbSet.RemoveRange(list);
            //todo: abstract save job to uint of work
            _dbContext.SaveChanges();
        }

        public Task<List<T>> GetAllAsync<T>() where T : class
        {
            return _dbContext.Set<T>().ToListAsync();
        }

        public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var theOne = _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
            return theOne;
        }      
    }
}
