using MongoDb.Common;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        string _connectionString;
        string _databaseName;
        static Dictionary<string, MongoClient> _mongoClients = new Dictionary<string, MongoClient>();
        IClientSessionHandle _session;
        MongoClient _mongoClient;

        private readonly List<Func<Task>> _commands;

        private IMongoDatabase _database;

        public IMongoDatabase Database
        {
            get
            {
                if (_database != null)
                    return _database;

                openConnection();

                return _database;
            }
        }

        public BaseUnitOfWork(IDatabaseSettings databaseSettings)
        {
            _connectionString = databaseSettings.ConnectionString;
            _databaseName = databaseSettings.DatabaseName;
            _commands = new List<Func<Task>>();
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
            while (_session != null && _session.IsInTransaction)
                Thread.Sleep(TimeSpan.FromMilliseconds(100));

            GC.SuppressFinalize(this);
        }

        public void InitTransaction()
        {
            openConnection();
        }

        public void CommitTransaction()
        {
            //if (_mongoClient == null)
            //    InitTransaction();
            //using (_session = _mongoClient.StartSession())
            //{
            //    _session.StartTransaction();

            //    var commandTasks = _commands.Select(c => c());

            //    _session.CommitTransaction();
            //}
        }

        public void RollbackTransaction()
        {
            _commands.Clear();
        }

        public Task InitTransactionAsync()
        {
            openConnection();
            return Task.FromResult(0);
        }

        public async Task CommitTransactionAsync()
        {
            //if (_mongoClient == null)
            //    InitTransaction();

            //using (_session = await _mongoClient.StartSessionAsync())
            //{
            //    _session.StartTransaction();

            //    var commandTasks = _commands.Select(c => c());

            //    await Task.WhenAll(commandTasks);

            //    await _session.CommitTransactionAsync();
            //}
        }

        public void AddCommand(Func<Task> func)
        {
            if (_mongoClient == null)
                InitTransaction();
            func();
            //_commands.Add(func);
        }

        public Task RollbackTransactionAsync()
        {
            _commands.Clear();
            return Task.FromResult(0);
        }
    }

}
