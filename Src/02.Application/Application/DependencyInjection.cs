using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apps.Sdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using Application.Common.Behaviours;
using Auth.Domain.Services;
using TreeDocs.Domain.Handlers;
using Auth.Domain.Handlers;
using Application.Common;
using Apps.Sdk.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(ISdkContainerBuilder builder, IConfigurationRoot config)
        {
            Auth.Domain.DependencyInjection.RegisterDependencies(builder, config);
            Auth.Domain.Persistence.MongoDb.DependencyInjection.RegisterRepositories(builder, config);
            TreeDocs.Repository.MongoDb.DependencyInjection.RegisterRepositories(builder, config);
            TreeDocs.Domain.DependencyInjection.RegisterDependencies(builder, config);
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(typeof(GetUserNodesHandler), typeof(CreateUserHandler));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidatorBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidateWithExceptionBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>)); // Not Working
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }

    }
}
