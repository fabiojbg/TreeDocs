using MongoDB.Bson;
using MongoDB.Driver;
using Pipelines.Sockets.Unofficial.Arenas;
using Repository.MongoDb.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDb.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : DbNodeEntity
    {
        IUnitOfWork _db;
        private IMongoCollection<TEntity> _internalCollection;

        IMongoCollection<TEntity> _collection => _internalCollection ??= _db.Database.GetCollection<TEntity>(typeof(TEntity).Name);

        public IQueryable<TEntity> Query => _collection.AsQueryable();

        public Repository(IUnitOfWork db)
        {
            _db = db;
        }

        public Task SaveChangesAsync()
        {
            return _db.CommitTransactionAsync();
        }

        public void SaveChanges()
        {
             _db.CommitTransaction();
        }

        public virtual async Task<ObjectId> CreateAsync(TEntity obj)
        {
            if( obj.Id == ObjectId.Empty)
                obj.Id = ObjectId.GenerateNewId();
            
            obj.CreatedOn = DateTime.UtcNow;
            obj.UpdatedOn = DateTime.UtcNow;

            if( _db.Session != null )
                await _collection.InsertOneAsync(_db.Session, obj);
            else
                await _collection.InsertOneAsync(obj);

            return obj.Id;
        }

        public virtual async Task<TEntity> GetByIdAsync(ObjectId id, params string[] fieldsToReturn)
        {
            if (id == ObjectId.Empty)
                return null;

            ProjectionDefinition<TEntity> projection = null;

            if( fieldsToReturn != null && fieldsToReturn.Length>0)
            {
                var projectionBuilder = Builders<TEntity>.Projection;
                projection = projectionBuilder.Combine(fieldsToReturn.Select(field => projectionBuilder.Include(field)));
            }

            if (projection == null)
            {
                IAsyncCursor<TEntity> data;
                if (_db.Session != null)
                    data = await _collection.FindAsync(_db.Session, Builders<TEntity>.Filter.Eq("_id", id));
                else
                    data = await _collection.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));

                return data.FirstOrDefault();
            }
            else
            {
                if (_db.Session != null)
                {
                    var data = _collection.Find(_db.Session, Builders<TEntity>.Filter.Eq("_id", id)).Project<TEntity>(projection);
                    return await data.FirstOrDefaultAsync();
                }
                else
                {

                    var queryProjection = _collection.Find(Builders<TEntity>.Filter.Eq("_id", id)).Project<TEntity>(projection);
                    return await queryProjection.FirstOrDefaultAsync();
                }
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetListAsync(ObjectId? folderId)
        {
            var data = await _collection.FindAsync(Builders<TEntity>.Filter.Eq(nameof(DbNodeEntity.ParentId), folderId));

            return data.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetListSelectAsync(ObjectId? folderid, params string[] fieldsToReturn)
        {
            var projectionBuilder = Builders<TEntity>.Projection;
            var projection = projectionBuilder.Combine(fieldsToReturn.Select(field => projectionBuilder.Include(field)));

            var data = await _collection.Find(Builders<TEntity>.Filter.Eq(nameof(DbNodeEntity.ParentId), folderid)).Project<TEntity>(projection).ToListAsync();

            return data;
        }

        public virtual async Task<IEnumerable<TEntity>> GetListExcludingFieldsAsync(ObjectId? folderId, params string[] fieldsToExclude)
        {
            var projectionBuilder = Builders<TEntity>.Projection;
            var projection = projectionBuilder.Combine(fieldsToExclude.Select(field => projectionBuilder.Exclude(field)));

            var data = await _collection.Find(Builders<TEntity>.Filter.Eq(nameof(DbNodeEntity.ParentId), folderId)).Project<TEntity>(projection).ToListAsync();

            return data;
        }


        public virtual async Task<IEnumerable<TEntity>> GetListExcludingFieldsAsync(String userId, params string[] fieldsToExclude)
        {
            var projectionBuilder = Builders<TEntity>.Projection;
            var projection = projectionBuilder.Combine(fieldsToExclude.Select(field => projectionBuilder.Exclude(field)));

            var data = await _collection.Find(Builders<TEntity>.Filter.Eq(nameof(DbNodeEntity.OwnerId), userId)).Project<TEntity>(projection).ToListAsync();

            return data;
        }

        public virtual async Task UpdateAsync(TEntity obj)
        {
            if (obj.Id == ObjectId.Empty)
                throw new Exception($"Error updating '{obj.GetType().Name}'  because it does not have de Id property");

            ReplaceOneResult actionResult;
            if (_db.Session != null)
                actionResult = await _collection.ReplaceOneAsync(_db.Session, Builders<TEntity>.Filter.Eq("_id", obj.Id), obj, new ReplaceOptions { IsUpsert = true });
            else
                actionResult = await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj, new ReplaceOptions { IsUpsert = true });

            if (!actionResult.IsAcknowledged || actionResult.ModifiedCount == 0)
                throw new Exception($"Error updating '{obj.GetType().Name}' with id='{obj.Id}' {actionResult.ToString()}");
        }

        public virtual async Task DeleteAsync(ObjectId id)
        {
            DeleteResult actionResult;
            if (_db.Session != null)
                actionResult = await _collection.DeleteOneAsync(_db.Session, Builders<TEntity>.Filter.Eq("_id", id));
            else
                actionResult = await _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));

            if (!actionResult.IsAcknowledged || actionResult.DeletedCount == 0)
                throw new Exception($"Error deleting object with id='{id}' {actionResult.ToString()}");
        }

        public async Task UpdatePropertyAsync(ObjectId objId, UpdateDefinition<TEntity> updateDefinition)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", objId);

            if (_db.Session != null)
                await _collection.FindOneAndUpdateAsync(_db.Session, filter, updateDefinition);
            else
                await _collection.FindOneAndUpdateAsync(filter, updateDefinition);
        }
    }
}




