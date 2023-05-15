using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Apps.Sdk.DependencyInjection;
using Apps.Sdk.Extensions;

namespace App.Sdk.DependencyInjection
{
    public class AutofacContainer : ISdkContainer, IDisposable
    {
        private bool disposedValue;
        protected IComponentContext _container;

        public AutofacContainer(IComponentContext componentContext)
        {
            _container = componentContext;
        }

        public ISdkContainer GetChildContainer()
        {
            if ((_container as ILifetimeScope) == null)
                throw new Exception("_container must be ILifetimeScope to get a ChildContainer");

            return new AutofacContainer((_container as ILifetimeScope).BeginLifetimeScope());
        }

        private IComponentContext scopeContainer
        {
            get
            {

                if (Object.ReferenceEquals(this, SdkDI.Resolver)) // se o Resolver atual for o Raiz, então usa o resolutor do sistema.
                {
                    ILifetimeScope scope = null;
                    try
                    {
                        scope = _container?.Resolve<IHttpContextAccessor>()?.HttpContext?.RequestServices.GetService<ILifetimeScope>();
                        if (scope != null)
                            return scope;
                    }
                    catch { }
                }
                return _container; // se bateu aqui é porque esse é um container filho usado para tarefas em background
            }
        }

        public T Resolve<T>()
        {
            var service = scopeContainer.Resolve<T>();

            return service;
        }

        public T Resolve<T>(string name)
        {
            var service = scopeContainer.ResolveNamed<T>(name);
            return service;
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            var types = scopeContainer.ComponentRegistry.Registrations.Where(r => typeof(T).IsAssignableFrom(r.Activator.LimitType))
                                                                       .DistinctBy(x => x.Activator.LimitType);

            var instances = types.Select(t =>
            {
                var namedService = t.Services?.ToList()?[0] as KeyedService;
                if (namedService != null)
                    return Resolve<T>(namedService.ServiceKey.ToString());

                return scopeContainer.Resolve(t.Activator.LimitType) as T;
            }).ToList();

            return instances;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if ((_container as ILifetimeScope) != null)
                    (_container as ILifetimeScope).Dispose();

                _container = null;
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AutofacContainerScope()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

