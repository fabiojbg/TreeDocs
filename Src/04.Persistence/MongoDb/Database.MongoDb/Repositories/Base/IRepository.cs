using MongoDB.Bson;
using MongoDB.Driver;
using Repository.MongoDb.DbModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.MongoDb.Repositories
{
    public interface IRepository<TEntity> : IBaseRepository<TEntity> where TEntity : DbNodeEntity
    {
        Task UpdatePropertyAsync(ObjectId objId, UpdateDefinition<TEntity> updateDefinition);
        Task<IEnumerable<TEntity>> GetListAsync(ObjectId? folderId);
        Task<IEnumerable<TEntity>> GetListSelectAsync(ObjectId? folderId, params string[] fieldsToReturn);
        Task<IEnumerable<TEntity>> GetListExcludingFieldsAsync(ObjectId? folderId, params string[] fieldsToExclude);
        Task<IEnumerable<TEntity>> GetListExcludingFieldsAsync(String userId, params string[] fieldsToExclude);

        IQueryable<TEntity> Query { get; }

        Task SaveChangesAsync();
        void SaveChanges();
    }
}
