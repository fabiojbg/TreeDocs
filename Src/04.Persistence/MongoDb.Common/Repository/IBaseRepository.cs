using MongoDb.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public interface IBaseRepository<TEntity>  where TEntity : DbEntity
    {
        Task<TEntity> GetByIdAsync(ObjectId id);
        Task<ObjectId> CreateAsync(TEntity model);
        Task UpdateAsync(TEntity model);
        Task DeleteAsync(ObjectId id);

        Task UpdatePropertyAsync(ObjectId objId, UpdateDefinition<TEntity> updateDefinition);

        IQueryable<TEntity> Query { get; }
    }
}
