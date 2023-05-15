using MongoDB.Bson;
using Repository.MongoDb.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDb.Repositories
{
    public interface IFileRepository<TEntity> : IBaseRepository<TEntity> where TEntity : DbFileModel
    {
        Task<TEntity> GetInfoByIdWithoutContentsAsync(ObjectId id);
    }
}
