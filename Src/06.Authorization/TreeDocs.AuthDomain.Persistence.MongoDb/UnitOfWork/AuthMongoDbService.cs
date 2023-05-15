using Apps.Sdk;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Threading.Tasks;
using Auth.Domain.Persistence.MongoDb;
using Auth.Domain.Repository;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain.Persistence.MongoDb
{
    public class AuthMongoDbService : IAuthDatabase
    {
        public IUserRepository _users;

        public IUserRepository Users => _users ??= SdkDI.Resolve<IUserRepository>();

        public IUnitOfWork _unitOfWork;
        public AuthMongoDbService(IUnitOfWork unitOfWork)             
        {
            _unitOfWork = unitOfWork;
        }

        public Task SaveChangesAsync()
        {
            return _unitOfWork.CommitTransactionAsync();
        }

        public void Dispose()
        {

            _unitOfWork.Dispose();
        }
    }
}
