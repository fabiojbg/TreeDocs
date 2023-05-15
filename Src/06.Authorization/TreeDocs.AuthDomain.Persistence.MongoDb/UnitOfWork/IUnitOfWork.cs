using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Auth.Domain.Persistence.MongoDb
{
    public interface IUnitOfWork
    {
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }

        void CommitTransaction();
        Task CommitTransactionAsync();
        void Dispose();
        void InitTransaction();
        Task InitTransactionAsync();
        void openConnection();
        void RollbackTransaction();
        Task RollbackTransactionAsync();
    }
}