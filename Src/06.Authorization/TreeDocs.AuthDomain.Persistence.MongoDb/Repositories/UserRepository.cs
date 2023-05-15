using Apps.Sdk.Extensions;
using Auth.Domain.MongoDb.DbModels;
using Auth.Domain.MongoDb.Extensions;
using Auth.Domain.Persistence.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.Domain.Repository;
using Auth.Domain.Entities;

namespace Auth.Domain.Persistence.MongoDb.Repositories
{
    public class UserRepository : IUserRepository
    {
        IRepository<DbUser> _dbUserRepository;

        public UserRepository(IRepository<DbUser> dbUserRepository)
        {
            _dbUserRepository = dbUserRepository;
        }

        public async Task<string> CreateUser(User user)
        {
            var dbUser = this.convertToDbModel(user, false);
            dbUser.CreatedOn = DateTime.UtcNow;
            dbUser.UpdatedOn = DateTime.UtcNow;

            var userId = await _dbUserRepository.CreateAsync(dbUser);

            user.SetId(userId.ToString());

            return user.Id.ToString();

        }

        public Task<User> GetUserByEmail(string email)
        {
            if (email.IsNullOrEmpty())
                return null;

            var user = _dbUserRepository.Query.Where(x => x.Email.ToLowerInvariant() == email.ToLowerInvariant()).FirstOrDefault();

            var domainUser = convertToDomainModel(user);

            return Task.FromResult(domainUser);
        }

        public async Task<User> GetUserById(string id)
        {
            if (id.IsNullOrEmpty())
                return null;

            var user = await _dbUserRepository.GetByIdAsync(id.ToObjectId());

            var domainUser = convertToDomainModel(user);

            return domainUser;
        }

        public async Task UpdateUserData(User user)
        {
            var dbUser = this.convertToDbModel(user, true);
            dbUser.UpdatedOn = DateTime.UtcNow;

            await _dbUserRepository.UpdateAsync(dbUser);
        }

        public async Task UpdateUserLastLogin(string userId, DateTime LastLogin)
        {

            var updateDef = Builders<DbUser>.Update.Set(x => x.LastLogin, LastLogin);
            await _dbUserRepository.UpdatePropertyAsync(userId.ToObjectId(), updateDef);
        }

        public async Task UpdateUserPassword(string userId, string hashedPassword)
        {

            var updateDef = Builders<DbUser>.Update.Set(x => x.HashedPassword, hashedPassword);
            await _dbUserRepository.UpdatePropertyAsync(userId.ToObjectId(), updateDef);
        }

        DbUser convertToDbModel(User user, bool validateId)
        {
            if (validateId && !ObjectId.TryParse(user.Id, out ObjectId objId))
                throw new InvalidOperationException("User id is invalid");

            var dbUser = new DbUser(user);

            return dbUser;
        }

        User convertToDomainModel(DbUser dbUser)
        {
            if (dbUser == null) return null;
            var domainUser = new User(dbUser.Id.ToString(), dbUser.Name, dbUser.Email, null, dbUser.HashedPassword, dbUser.Roles);
            domainUser.SetId( dbUser.Id.ToString() );
            return domainUser;
        }
    }
}
