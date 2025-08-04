using Apps.Sdk;
using TreeNotes.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Repository.MongoDb.DbModels;
using Repository.MongoDb.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TreeNotes.Domain.Repositories;
using Repository.MongoDb;
using Apps.Sdk.DependencyInjection;

namespace TreeNotes.Repository.MongoDb
{
    public static class DependencyInjection
    {
        public static void RegisterRepositories(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            builder.RegisterSingleton<IDatabaseSettings, DatabaseSettings>();
            builder.RegisterScoped<IAppDatabase, MongoDbService>();
            builder.RegisterScoped<IUnitOfWork, UnitOfWork>();
            builder.RegisterScoped<IRepository<DbUserNode>, Repository<DbUserNode>>();
            builder.RegisterScoped<IFileRepository<DbNodeContents>, FileRepository<DbNodeContents>>();
            builder.RegisterScoped<IUserNodeRepository, UserNodeRepository>();
        }

    }
}
