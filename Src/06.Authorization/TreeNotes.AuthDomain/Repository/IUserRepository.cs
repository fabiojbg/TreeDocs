using Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.Repository
{
    public interface IUserRepository
    {
        Task<string> CreateUser(User user);

        Task<User> GetUserByEmail(string email);

        Task<User> GetUserById(string id);

        Task UpdateUserData(User user);

        Task UpdateUserLastLogin(string userId, DateTime LastLogin);

        Task UpdateUserPassword(string userId, string hashedPassword);
    }
}
