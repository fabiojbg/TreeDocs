using System;
using System.Text;

namespace Apps.Sdk.DependencyInjection
{

    public interface ISdkContainerBuilder
    {
        void RegisterSingleton<T, Y>() where Y : T;
        void RegisterSingleton<T, Y>(string name) where Y : T;
        void RegisterSingleton<T>(Func<ISdkContainer, T> func);

        void RegisterScoped<T, Y>() where Y : T;
        void RegisterScoped<T, Y>(string name) where Y : T;
        void RegisterScoped<T>(Func<ISdkContainer, T> func);

        void RegisterTransient<T, Y>() where Y : T;
        void RegisterTransient<T, Y>(string name) where Y : T;
        void RegisterTransient<T>(Func<ISdkContainer, T> func);

        void RegisterInstance<T>(T instance) where T : class;
        void RegisterInstance<T>(string name, T instance) where T : class;
    }

}
