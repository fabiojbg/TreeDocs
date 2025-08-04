using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TreeNotes.Domain.Repositories;

namespace TreeNotes.Domain.Repositories
{
    public interface IAppDatabase : IDisposable
    {
        Task SaveChangesAsync();

        IUserNodeRepository UserNodes { get; }
    }
}
