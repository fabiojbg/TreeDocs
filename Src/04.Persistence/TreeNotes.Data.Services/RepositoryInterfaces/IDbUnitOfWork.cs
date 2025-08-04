using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treenotes.Data.Services
{
    public interface IDbUnitOfWork : IDisposable
    {
        void InitTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Task InitTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
