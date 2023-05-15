using Auth.Domain.MongoDb.DbModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.Persistence.MongoDb
{
    public interface IBaseRepository<TEntity>  where TEntity : DbEntity
    {
        Task<TEntity> GetByIdAsync(ObjectId id);
        Task<ObjectId> CreateAsync(TEntity model);
        Task UpdateAsync(TEntity model);
        Task DeleteAsync(ObjectId id);

    }
}
