using App.Sdk.DependencyInjection;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Sdk.DependencyInjection
{
    public static class SdkDI
    {
        public static ISdkContainer Resolver { get; private set; }

        public static void SetGlobalResolver(ISdkContainer container)
        {
            Resolver = container;
        }

        public static ISdkContainer GetChildContainer()
        {
            return Resolver.GetChildContainer();
        }

        public static T Resolve<T>()
        {
            return Resolver.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Resolver.Resolve<T>(name);
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Resolver.ResolveAll<T>();
        }

        public static ISdkContainer GetContainer(IComponentContext componentContext)
        {
            return  new AutofacContainer(componentContext);
        }


    }
}
