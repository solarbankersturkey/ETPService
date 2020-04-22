using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CustomerService.Model
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Create(TEntity obj);
        void Update(TEntity obj);
        void Delete(string id);
        Task<TEntity> Get(string id);
        Task<List<TEntity>> Get(FilterDefinition<TEntity> filter);
        Task<IEnumerable<TEntity>> Get();
    }
}
