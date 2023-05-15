using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public interface IBaseUnitOfWork : IDisposable
    {
        IMongoDatabase Database { get; }
        void AddCommand(Func<Task> func);
        void CommitTransaction();
        Task CommitTransactionAsync();
        void InitTransaction();
        Task InitTransactionAsync();
        void openConnection();
        void RollbackTransaction();
        Task RollbackTransactionAsync();
    }
}