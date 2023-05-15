using Autofac;
using Autofac.Core.Activators;
using Apps.Sdk;
using Apps.Sdk.DependencyInjection;
using System;

namespace Apps.Sdk.DependencyInjection
{
    public class AutofacContainerBuilder : ISdkContainerBuilder
    { 
        Autofac.ContainerBuilder _builder;

        public AutofacContainerBuilder(Autofac.ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterSingleton<T, Y>() where Y : T
        {
            _builder.RegisterType<Y>().As<T>().SingleInstance();
        }

        public void RegisterSingleton<T>(Func<ISdkContainer, T> func)
        {
            _builder.Register(c => func(SdkDI.GetContainer(c))).As<T>().SingleInstance();
        }

        public void RegisterScoped<T, Y>() where Y : T
        {
            _builder.RegisterType<Y>().As<T>().InstancePerLifetimeScope();
        }

        public void RegisterSingleton<T, Y>(string name) where Y : T
        {
            _builder.RegisterType<Y>().Named<T>(name).SingleInstance();
        }

        public void RegisterScoped<T, Y>(string name) where Y : T
        {
            _builder.RegisterType<Y>().Named<T>(name).InstancePerLifetimeScope();
        }

        public void RegisterTransient<T, Y>(string name) where Y : T
        {
            _builder.RegisterType<Y>().Named<T>(name).InstancePerDependency();
        }

        public void RegisterScoped<T>(Func<ISdkContainer, T> func)
        {
            _builder.Register(c => func(SdkDI.GetContainer(c))).As<T>().InstancePerLifetimeScope();
        }

        public void RegisterTransient<T, Y>() where Y : T
        {
            _builder.RegisterType<Y>().As<T>().InstancePerDependency();
        }

        public void RegisterTransient<T>(Func<ISdkContainer, T> func)
        {
            _builder.Register(c => func(SdkDI.GetContainer(c))).As<T>().InstancePerDependency();
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            _builder.RegisterInstance(instance).As<T>();
        }

        public void RegisterInstance<T>(string name, T instance) where T : class
        {
            _builder.RegisterInstance<T>(instance).Named<T>(name);
        }

    }

}
