using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TreeDocs.Domain.Repositories;

namespace TreeDocs.Domain.Repositories
{
    public interface IAppDatabase : IDisposable
    {
        Task SaveChangesAsync();

        IUserNodeRepository UserNodes { get; }
    }
}
