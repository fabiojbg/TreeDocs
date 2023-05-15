using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.MongoDb
{
    public class UnitOfWork : IUnitOfWork
    {
        string _connectionString;
        string _databaseName;
        static bool _transactionSupported = false;
        static Dictionary<string, MongoClient> _mongoClients = new Dictionary<string, MongoClient>();
        MongoClient _mongoClient;
        ILogger<UnitOfWork> _logger;

        public IClientSessionHandle Session { get; private set; }

        private IMongoDatabase _database;

        public IMongoDatabase Database
        {
            get
            {
                if (_database != null)
                    return _database;

                InitTransaction();

                return _database;
            }
        }


        public UnitOfWork(IDatabaseSettings databaseSettings, ILogger<UnitOfWork> logger)
        {
            _connectionString = databaseSettings.ConnectionString;
            _databaseName = databaseSettings.DatabaseName;
            _logger = logger;
        }


        public void openConnection()
        {
            if (_mongoClient != null)
                return;

            if (_mongoClients.ContainsKey(_connectionString + _databaseName))
                _mongoClient = _mongoClients[_connectionString + _databaseName];
            else
            {
                _mongoClient = new MongoClient(_connectionString);
                _mongoClients.Add(_connectionString + _databaseName, _mongoClient);
            }
            if (_mongoClient != null)
                _database = _mongoClient.GetDatabase(_databaseName);
        }

        public void Dispose()
        {
            try
            {
                RollbackTransaction();
            }
            catch(Exception ex)
            {
                var a = ex;
            }
            while (Session != null && Session.IsInTransaction)
                Thread.Sleep(TimeSpan.FromMilliseconds(100));

            GC.SuppressFinalize(this);
        }

        public void InitTransaction()
        {
            openConnection();
            Session = null;
            if (_transactionSupported)
            {
                try
                {
                    Session = _mongoClient.StartSession();
                    Session.StartTransaction(new TransactionOptions(ReadConcern.Local, ReadPreference.Primary, WriteConcern.WMajority));
                }
                catch (NotSupportedException ex) // transaction not supported
                {
                    _transactionSupported = false;
                    _logger.LogError(ex, "Detected connection does not support transactions. Transactions is turned off for now on");
                }
            }

        }

        public async Task InitTransactionAsync()
        {
            openConnection();
            Session = null;
            if (_transactionSupported)
            {
                try
                {
                    Session = await _mongoClient.StartSessionAsync();
                }
                catch(Exception ex)
                {
                    _transactionSupported = false;
                    _logger.LogError(ex, "Error opening transaction. Transactions support is turned off now");
                }
            }
        }

        public void CommitTransaction()
        {
            if (Session != null)
                Session.CommitTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            if( Session != null)
                await Session.CommitTransactionAsync();
        }

        public void RollbackTransaction()
        {
            try
            {
                if (Session != null)
                    Session.AbortTransaction();
            }
            catch { }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (Session != null)
                    await Session.AbortTransactionAsync();
            }
            catch { }
        }

    }

}
