using Apps.Sdk;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Repository.MongoDb.DbModels;
using System;
using System.Threading.Tasks;
using TreeDocs.Domain.Repositories;

namespace Repository.MongoDb
{
    public class MongoDbService : IAppDatabase
    {
        public IUserNodeRepository UserNodes { get; private set; }
        public IUnitOfWork _unitOfWork;
        public MongoDbService(IUnitOfWork unitOfWork, 
                              IUserNodeRepository userNodeRepository)             
        {
            _unitOfWork = unitOfWork;
            UserNodes = userNodeRepository;
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
