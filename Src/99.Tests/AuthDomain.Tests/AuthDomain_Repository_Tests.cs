using Auth.Domain.Entities;
using Auth.Domain.MongoDb.DbModels;
using Auth.Domain.Persistence.MongoDb;
using Auth.Domain.Persistence.MongoDb.Repositories;
using Auth.Domain.MongoDb.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Auth.Domain.Services;
using Domain.Shared;
using Domain.Shared.Validations;
using Auth.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Collections.Concurrent;
using Auth.Domain.RequestsResponses;
using Auth.Domain.Handlers;

namespace Auth.Domain.Tests
{
    public class DatabaseFixture : IDisposable
    {
        Mock<IDatabaseSettings> _dbSettingsMock;
        public IUnitOfWork UnitOfWorkForTests;

        static string _connectionString = "mongodb://admin:12345678a@localhost:27017";
        static string _database = "UsersTest";

        public const string AdminId = "5f8d242a2f47081d11ace3b7";
        public const string AdminName = "Administrator";
        public const string AdminEmail = "Admin@admin.com";
        public const string AdminEmailLowercase = "admin@admin.com";
        public const string UserId = "5f8d24fc08299ef1408c9eb2";
        public const string UserName = "Common User";
        public const string UserEmail = "AppUser@user.com";
        public const string UserEmailLowercase = "appuser@user.com";
        public const string Password = "123456";

        Repository<DbUser> _userRepository;

        public DatabaseFixture()
        {
            _dbSettingsMock = new Mock<IDatabaseSettings>();
            _dbSettingsMock.Setup(x => x.ConnectionString).Returns(_connectionString);
            _dbSettingsMock.Setup(x => x.DatabaseName).Returns(_database);

            UnitOfWorkForTests = new UnitOfWork(_dbSettingsMock.Object, new Mock<ILogger<UnitOfWork>>().Object);

            UnitOfWorkForTests.Database.DropCollection(nameof(DbUser));

            _userRepository = new Repository<DbUser>(UnitOfWorkForTests);

            CreateDbUser(AdminId, AdminName, Password, AdminEmailLowercase, Constants.ADMIN_USER_ROLE);
            CreateDbUser(UserId, UserName, Password, UserEmailLowercase, Constants.APPUSER_ROLE);

        }

        public void CreateDbUser(string userId, string name, string password, string email, params string[] roles)
        {
            _userRepository.CreateAsync(new DbUser() { 
                Id = userId.ToObjectId(),
                Name = name,
                Email = email,
                HashedPassword = User.GetHashedPassword(userId, password),
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                Roles = roles
            }).Wait();
        }

        public void Dispose()
        {
            try
            {
                _userRepository.DeleteAsync(AdminId.ToObjectId()).Wait();
                _userRepository.DeleteAsync(UserId.ToObjectId()).Wait();
            }
            catch { }
            
            UnitOfWorkForTests.Dispose();
        }
    }

    public class AuthDomain_Repository_Tests : IClassFixture<DatabaseFixture>, IDisposable
    {
        DatabaseFixture _db;
        Repository<DbUser> _dbRepository;
        UserRepository _userRepository;
        Mock<IAuthDatabase> _authDbMock;
        Mock<ILogger<AuthenticationUserHandler>> _loggerMock;
        ConcurrentQueue<string> _idsToRemove;

        public AuthDomain_Repository_Tests(DatabaseFixture db)
        {
            // Mocks
            _authDbMock = new Mock<IAuthDatabase>();
            _loggerMock = new Mock<ILogger<AuthenticationUserHandler>>();

            // real objects
            _db = db;
            _dbRepository = new Repository<DbUser>(_db.UnitOfWorkForTests);
            _userRepository = new UserRepository(_dbRepository);

            // direct mocks to real objects
            _authDbMock.Setup(x => x.Users).Returns(_userRepository);

            // list of items created during test to be removed at the end
            _idsToRemove = new ConcurrentQueue<string>();
        }

        public void Dispose()
        {
            deleteObjectsCreatedByTests();
        }

        private void deleteObjectsCreatedByTests()
        {
            string idToRemove;

            while( _idsToRemove.TryDequeue(out idToRemove) )
            {
                try
                {
                    _dbRepository.DeleteAsync(idToRemove.ToObjectId()).Wait();
                }
                catch { }
            }
        }

        private ILoggedUserService getLoggedUserService(string loggedUserId)
        {
            var mockAppUserService = new Mock<IAppUserService>();
            // make a logged user a admin or common user
            mockAppUserService.Setup(x => x.GetLoggedUserId()).Returns(loggedUserId);
            // recreate loggerUserService to a clean logged user data filled by previous calls
            return new LoggedUserService(mockAppUserService.Object, _userRepository, new Mock<ILogger<LoggedUserService>>().Object);
        }

        [Theory]
        [InlineData("Other User", "otherUser@user.com", "123456", "AppUser", DatabaseFixture.UserId)] // user can create an account
        [InlineData("Other User", "otherUser@user.com", "123456", "AppUser", DatabaseFixture.AdminId)] // Admin can create an user account
        [InlineData("Administrator2", "admin2@admin.com", "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.AdminId)] // Admin can create other admin account
        public async Task CreateUserHandler_Valid(string name, string email, string password, string proles, string loggedUserId)
        {
            var roles = proles?.Split(',')?.Select(x => x.Trim())?.ToArray();

            var request = new CreateUserRequest()
            {
                Name = name,
                Email = email,
                Roles = roles,
                Password = password
            };

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new CreateUserHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            if (result?._Data != null)
                _idsToRemove.Enqueue(result._Data.Id);

            Assert.Equal(RequestResultType.Success, result._Result);
            Assert.NotNull(result._Data.Id);
            Assert.Equal(email.ToLowerInvariant(), result._Data.Email);
            Assert.Equal(name, result._Data.Name);
            Assert.True(roles==null || roles.All(x => result._Data.Roles.Contains(x)));
        }

        [Theory]
        [InlineData("Administrator2", "admin2@admin.com", "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.UserId, RequestResultType.Unauthorized)]  // user not admin trying to create admin
        [InlineData("Ad", "admin@admin.com", "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.AdminId, RequestResultType.InvalidRequest, "Name")]     // invalid name
        [InlineData("Administrator", "admin-admin.com", "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.AdminId, RequestResultType.InvalidRequest)]  //  invalid email
        [InlineData("Administrator", DatabaseFixture.AdminEmail, "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.AdminId, RequestResultType.InvalidRequest)] // user already exists
        public async Task CreateUserHandler_Invalid(string name, string email, string password, string proles, string loggedUserId, RequestResultType expectedResult, params string[] expectedNotifications)
        {
            var roles = proles.Split(',').Select(x => x.Trim()).ToArray();

            var request = new CreateUserRequest()
            {
                Name = name,
                Email = email,
                Roles = roles,
                Password = password
            };

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new CreateUserHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            if (result?._Data != null)
                _idsToRemove.Enqueue(result._Data.Id);

            Assert.Equal(expectedResult, result._Result);
            Assert.Null(result._Data);
            if (expectedNotifications?.Any() == true)
                Assert.All(expectedNotifications, item => result._Notifications?.Any(y => y.Property == item));
        }


        [Theory]
        [InlineData(DatabaseFixture.AdminEmail, DatabaseFixture.Password, DatabaseFixture.AdminName)] // test case insensitivity to email
        [InlineData(DatabaseFixture.AdminEmailLowercase, DatabaseFixture.Password, DatabaseFixture.AdminName)]
        [InlineData(DatabaseFixture.UserEmail, DatabaseFixture.Password, DatabaseFixture.UserName)]// test case insensitivity to email
        [InlineData(DatabaseFixture.UserEmailLowercase, DatabaseFixture.Password, DatabaseFixture.UserName)]
        public async void AuthenticationUserHandler_Valid(string userEmail, string password, string userName)
        {
            var handler = new AuthenticationUserHandler(_authDbMock.Object, _loggerMock.Object);

            var request = new AuthenticateUserRequest()
            {
                UserEmail = userEmail,
                Password = password
            };

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(RequestResultType.Success, result._Result);
            Assert.Null(result._Notifications);
            Assert.Equal(userEmail.ToLowerInvariant(), result._Data.Email);
            Assert.Equal(userName, result._Data.Name);
        }

        [Theory]
        [InlineData(DatabaseFixture.AdminEmail, "1234567", RequestResultType.Unauthorized)] // password should be 123456
        [InlineData("admin1234@admin.com", "123456", RequestResultType.Unauthorized)] // user does not exist
        public async void AuthenticationUserHandler_Invalid(string userEmail, string password, RequestResultType expectedResult)
        {
            var handler = new AuthenticationUserHandler(_authDbMock.Object, _loggerMock.Object);

            var request = new AuthenticateUserRequest()
            {
                UserEmail = userEmail,
                Password = password
            };

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(expectedResult, result._Result);
            Assert.Null(result._Data);
        }

        [Theory]
        [InlineData(DatabaseFixture.AdminEmail, DatabaseFixture.AdminId)]  // admin tries to get its own data
        [InlineData("admin2@admin.com", DatabaseFixture.AdminId)]                   // admin tries to get another admin data
        [InlineData(DatabaseFixture.UserEmail, DatabaseFixture.UserId)]    // user tries to get its own data
        [InlineData(DatabaseFixture.UserEmail, DatabaseFixture.AdminId)]   // admin tries to get user data
        public async void GetUserHandler_Valid(string userEmail, string loggedUserId)
        {

            var request = new GetUserRequest()
            {
                Email = userEmail
            };

            await CreateUserHandler_Valid("Admin 2", "admin2@admin.com", "123456", Constants.ADMIN_USER_ROLE, DatabaseFixture.AdminId);

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new GetUserHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(RequestResultType.Success, result._Result);
            Assert.Null(result._Notifications);
            Assert.Equal(userEmail.ToLowerInvariant(), result._Data.Email);
        }


        [Theory]
        [InlineData(DatabaseFixture.AdminEmail, DatabaseFixture.UserId, RequestResultType.Unauthorized)] // user tries to get admin data
        [InlineData("user2@user.com", DatabaseFixture.UserId, RequestResultType.Unauthorized)] // user tries to get other user data
        [InlineData("inexisting@user.com", DatabaseFixture.UserId, RequestResultType.Unauthorized)] // user tries to get inexisting user
        [InlineData("inexisting@user.com", DatabaseFixture.AdminId, RequestResultType.ObjectNotFound)] // admin tries to get inexisting user
        [InlineData("", DatabaseFixture.AdminId, RequestResultType.InvalidRequest)] // admin tries to get user with an invalid email
        public async void GetUserHandler_Invalid(string userEmail, string loggedUserId, RequestResultType expectedResult)
        {

            var request = new GetUserRequest()
            {
                Email = userEmail
            };

            await CreateUserHandler_Valid("User 2", "user2@user.com", "123456", Constants.APPUSER_ROLE, DatabaseFixture.AdminId);

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new GetUserHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(expectedResult, result._Result);
            Assert.Null(result._Data);
            Assert.Null(result._Notifications);
        }

        [Theory]
        [InlineData(DatabaseFixture.AdminId, "5f9516138271a9c8c87f1eb5", "Admin 2", "admin2@admin.com", "Admin,NewRole")] // admin changes admin
        [InlineData(DatabaseFixture.AdminId, "5f9515f38271a9c8c87f1eb4", "Admin 3", "admin3@admin.com", "Admin,NewRole")] // admin changes user to admin
        [InlineData("5f9515f38271a9c8c87f1eb4", "5f9515f38271a9c8c87f1eb4", "User 3", "user3@admin.com", "Admin,NewRole")] // user changes its own data
        public async void UpdateUserDataHandler_Valid(string loggedUserId, string changingUserId, string newName, string newEmail, string newRoles)
        {
            var newRolesList = newRoles?.Split(',')?.Select(x => x.Trim())?.ToList();

            var request = new UpdateUserDataRequest()
            {
                UserId = changingUserId,
                Email = newEmail,
                Name = newName,
                Roles = newRolesList
            };

            _db.CreateDbUser("5f9516138271a9c8c87f1eb5", "Administrator 2", "123456", "admin2@admin.com", Constants.ADMIN_USER_ROLE);
            _db.CreateDbUser("5f9515f38271a9c8c87f1eb4", "User 2", "123456", "user2@admin.com", Constants.APPUSER_ROLE);
            _idsToRemove.Enqueue("5f9516138271a9c8c87f1eb5");
            _idsToRemove.Enqueue("5f9515f38271a9c8c87f1eb4");

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new UpdateUserDataHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(RequestResultType.Success, result._Result);
            Assert.NotNull(result._Data);
            Assert.Null(result._Notifications);
            Assert.Equal(newName, result._Data.Name);
            Assert.Equal(newEmail.ToLowerInvariant(), result._Data.Email);
            if (loggedUserService.HasRole(Constants.ADMIN_USER_ROLE))
                Assert.True(newRolesList == null || newRolesList.All(x => result._Data.Roles.Contains(x)));
            else
                Assert.DoesNotContain( Constants.ADMIN_USER_ROLE, result._Data.Roles); // regular user cannot make itself an admin
        }

        [Theory]
        [InlineData(DatabaseFixture.UserId, "5f9516138271a9c8c87f1eb4", "User 3", "user3@admin.com", "Admin,NewRole", RequestResultType.Unauthorized, null)] // user tries to change another user data
        [InlineData("5f9516138271a9c8c87f1eb4", "5f9516138271a9c8c87f1eb4", "User 3", "", "Admin,NewRole", RequestResultType.InvalidRequest, null)] // user tries to change to an empty email
        [InlineData("5f9515f38271a9c8c87f1eb4", "5f9515f38271a9c8c87f1eb4", "User 3", "invalid-email", "Admin,NewRole", RequestResultType.InvalidRequest, "Email")] // user tries to change to an invalid email
        public async void UpdateUserDataHandler_Invalid(string loggedUserId, 
                                                        string changingUserId, 
                                                        string newName, 
                                                        string newEmail, 
                                                        string newRoles, 
                                                        RequestResultType expectedResult,
                                                        string expectedNotifications)
        {
            var expectedNotificationsList = expectedNotifications?.Split(',').Select(x => x.Trim()).ToList();
            var newRolesList = newRoles?.Split(',')?.Select(x => x.Trim())?.ToList();

            var request = new UpdateUserDataRequest()
            {
                UserId = changingUserId,
                Email = newEmail,
                Name = newName,
                Roles = newRolesList
            };

            _db.CreateDbUser("5f9516138271a9c8c87f1eb5", "Administrator 2", "123456", "admin2@admin.com", Constants.ADMIN_USER_ROLE);
            _db.CreateDbUser("5f9515f38271a9c8c87f1eb4", "User 2", "123456", "user2@admin.com", Constants.APPUSER_ROLE);
            _idsToRemove.Enqueue("5f9516138271a9c8c87f1eb5");
            _idsToRemove.Enqueue("5f9515f38271a9c8c87f1eb4");

            var loggedUserService = getLoggedUserService(loggedUserId);
            var handler = new UpdateUserDataHandler(_authDbMock.Object, loggedUserService);

            var result = await handler.Handle(request, new System.Threading.CancellationToken());

            Assert.Equal(expectedResult, result._Result);
            Assert.Null(result._Data);
            if (expectedNotifications == null)
                Assert.Null(result._Notifications);
            else
            {
                Assert.NotNull(result._Notifications);
                Assert.All(expectedNotificationsList, item => result._Notifications.Any(x => x.Property == item));
            }
        }


    }
}
