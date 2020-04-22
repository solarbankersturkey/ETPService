using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomerService.Model
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IMongoUserDBContext _mongoContext;
        protected IMongoCollection<TEntity> _dbCollection;

        protected BaseRepository(IMongoUserDBContext context)
        {
            _mongoContext = context;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name.ToLower());
        }

        public async void Create(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(typeof(TEntity).Name + " object is null");
            }
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            
            await _dbCollection.InsertOneAsync(obj);

            //INSERTED ID
            var id = obj.GetType().GetProperty("Id").GetValue(obj, null);

        }

        public void Delete(string id)
        {

             var objectId = new ObjectId(id);
             _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId));
        }

        public async Task<TEntity> Get(string id)
        {

            var objectId = new ObjectId(id);

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            var all = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }

        public async Task<List<TEntity>> Get(FilterDefinition<TEntity> filter)
        {
            var result = await _dbCollection.FindAsync<TEntity>(filter);
            return await result.ToListAsync();
        }

        public async void Update(TEntity obj)
        {
            await _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetType().GetProperty("Id").GetValue(obj, null)), obj);
        }


        //public async Task Create(TEntity obj)
        //{
        //    if (obj == null)
        //    {
        //        throw new ArgumentNullException(typeof(TEntity).Name + " object is null");
        //    }
        //    _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
        //    await _dbCollection.InsertOneAsync(obj);
        //}

        //public void Delete(string id)
        //{

        //    var objectId = new ObjectId(id);
        //    _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId));

        //}



    }
}
