using MongoDb.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : DbEntity
    {
        IBaseUnitOfWork _db;
        private IMongoCollection<TEntity> _internalCollection;

        IMongoCollection<TEntity> _collection => _internalCollection ??= _db.Database.GetCollection<TEntity>(typeof(TEntity).Name);

        public IQueryable<TEntity> Query => _collection.AsQueryable();

        public BaseRepository(IBaseUnitOfWork db)
        {
            _db = db;
        }

        public virtual Task<ObjectId> CreateAsync(TEntity obj)
        {
            if( obj.Id == null || obj.Id == ObjectId.Empty)
                obj.Id = ObjectId.GenerateNewId();
            
            obj.CreatedOn = DateTime.UtcNow;
            obj.UpdatedOn = DateTime.UtcNow;

            _db.AddCommand(async () => await _collection.InsertOneAsync(obj));
            return Task.FromResult(obj.Id);
        }

        public virtual async Task<TEntity> GetByIdAsync(ObjectId id)
        {
            if (id == null)
                return null;

            var data = await _collection.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            return data.FirstOrDefault(); 
        }

        public virtual Task UpdateAsync(TEntity obj)
        {
            if (obj.Id == null)
                throw new Exception($"Error updating '{obj.GetType().Name}'  because it does not have de Id property");

            _db.AddCommand(async () =>
            {
                ReplaceOneResult actionResult = await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj, new ReplaceOptions { IsUpsert = true });
                if (!actionResult.IsAcknowledged || actionResult.ModifiedCount == 0)
                    throw new Exception($"Error updating '{obj.GetType().Name}' with id='{obj.Id}' {actionResult.ToString()}");
            });

            return Task.FromResult(0);
        }

        public virtual Task DeleteAsync(ObjectId id)
        {
            _db.AddCommand(async () =>
           {
               DeleteResult actionResult = await _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
               if (!actionResult.IsAcknowledged || actionResult.DeletedCount == 0)
                   throw new Exception($"Error deleting object with id='{id}' {actionResult.ToString()}");
           });

            return Task.FromResult(0);
        }

        public async Task UpdatePropertyAsync(ObjectId objId, UpdateDefinition<TEntity> updateDefinition)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", objId);

            await _collection.FindOneAndUpdateAsync(filter, updateDefinition);
        }
    }
}




