using MongoDB.Bson;
using MongoDB.Driver;
using Repository.MongoDb.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDb.Repositories
{
    public interface IBaseRepository<TEntity>  where TEntity : DbNodeEntity
    {
        Task<TEntity> GetByIdAsync(ObjectId id);
        Task<ObjectId> CreateAsync(TEntity model);
        Task UpdateAsync(TEntity model);
        Task DeleteAsync(ObjectId id);

    }
}
