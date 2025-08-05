using Domain.Shared;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Collections;

namespace Audit.Persistence.MongoDb
{
    public class AuditTrail : IAuditTrail
    {
        IMongoClient _client;
        IMongoDatabase _database;
        string _serviceId;

        public AuditTrail(IConfiguration config)
        {
            var connectionString = config.GetValue<string>("AuditDatabase:ConnectionString");
            if (String.IsNullOrWhiteSpace(connectionString))
                connectionString = config.GetValue<string>("TreeNotesDb:ConnectionString"); // User DefaultTreeNotesDb if authentication Db not defined

            var dbName = config.GetValue<string>("AuditDatabase:DatabaseName") ?? "TreeNotesDb";
            _serviceId = config.GetValue<string>("AuditDatabase:ServiceId") ?? "Audit";

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(dbName);
        }

        public void InsertEntry(string message, string userName, string userId, string userIP, string messageDetails)
        {
            IMongoCollection<AuditEntry> entriesCollection = _database.GetCollection<AuditEntry>(_serviceId);

            var entry = new AuditEntry()
            {
                CreatedOn = DateTime.UtcNow,
                Message = message ?? "--",
                MessageDetails = messageDetails ?? "",
                UserId = userId ?? "--",
                UserIP = userIP ?? "--",
                UserName = userName ?? "--",
            };

            entriesCollection.InsertOne(entry);
        }
    }
}