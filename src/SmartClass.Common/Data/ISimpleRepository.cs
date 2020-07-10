using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartClass.Common.Data
{
    public interface ISimpleRepository : ISimpleRepositoryAsync
    {
        IList<T> GetAll<T>() where T : class;
        IQueryable<T> Query<T>() where T : class;
        T Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        T Get<T>(object id) where T : class;
        void Add<T>(IEnumerable<T> entities) where T : class;

        void Update<T>(IEnumerable<T> entities) where T : class;        
        void Delete<T>(IEnumerable<T> entities) where T : class;
        void Truncate<T>() where T : class;
    }

    public interface ISimpleRepositoryAsync
        //:IMyScoped
    {
        Task<List<T>> GetAllAsync<T>() where T : class;
        Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        #region async

        //https://stackoverflow.com/questions/42034282/are-there-dbset-updateasync-and-removeasync-in-net-core
        //ToListAsync exists because it actually causes EF to head off to the data store to retrieve the data.This may take some time, hence why you can call it asynchronously.
        //AddAsync however, only begins tracking an entity but won't actually send any changes to the database until you call SaveChanges or SaveChangesAsync.
        //You shouldn't really be using this method unless you know what you're doing.
        //The reason the async version of this method exists is explained in the docs:
        //This method is async only to allow special value generators,
        //such as the one used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
        //to access the database asynchronously.For all other cases the non async method should be used.
        //Task AddAsync<T>(IEnumerable<T> entities) where T : class;
        //Task UpdateAsync<T>(IEnumerable<T> entities) where T : class;
        //Task DeleteAsync<T>(IEnumerable<T> entities) where T : class;

        #endregion
    }  
}
