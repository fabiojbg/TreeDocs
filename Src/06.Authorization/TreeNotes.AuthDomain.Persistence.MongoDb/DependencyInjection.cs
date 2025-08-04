using Apps.Sdk;
using Microsoft.Extensions.Configuration;
using Auth.Domain.Repository;
using Auth.Domain.MongoDb.DbModels;
using Auth.Domain.Persistence.MongoDb.Repositories;
using Apps.Sdk.DependencyInjection;

namespace Auth.Domain.Persistence.MongoDb
{
    public static class DependencyInjection
    {
        public static void RegisterRepositories(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            builder.RegisterSingleton<IDatabaseSettings, DatabaseSettings>();
            builder.RegisterScoped<IAuthDatabase, AuthMongoDbService>();
            builder.RegisterScoped<IUnitOfWork, UnitOfWork>();
            builder.RegisterScoped<IRepository<DbUser>, Repository<DbUser>>();
            builder.RegisterScoped<IUserRepository, UserRepository>();
        }

    }
}
