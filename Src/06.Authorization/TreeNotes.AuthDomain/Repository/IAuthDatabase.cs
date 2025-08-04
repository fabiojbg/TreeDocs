using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.Repository
{
    public interface IAuthDatabase : IDisposable
    {
        Task SaveChangesAsync();

        IUserRepository Users { get; }
    }
}
