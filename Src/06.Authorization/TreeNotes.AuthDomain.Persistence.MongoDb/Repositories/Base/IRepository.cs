using Auth.Domain.MongoDb.DbModels;
using Auth.Domain.Persistence.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Domain.Persistence.MongoDb
{
    public interface IRepository<TEntity> : IBaseRepository<TEntity> where TEntity : DbEntity
    {
        Task UpdatePropertyAsync(ObjectId objId, UpdateDefinition<TEntity> updateDefinition);

        IQueryable<TEntity> Query { get; }
    }
}
