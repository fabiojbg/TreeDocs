using System;
using System.Collections.Generic;

namespace Apps.Sdk.DependencyInjection
{
    public interface ISdkContainer : IDisposable
    {
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<T> ResolveAll<T>() where T : class;
        ISdkContainer GetChildContainer();
    }

}
