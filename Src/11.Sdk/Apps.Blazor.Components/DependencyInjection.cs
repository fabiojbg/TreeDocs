using Apps.Blazor.Components.Navigation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Blazor.Components
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppsBlazorComponents(this IServiceCollection servicesCollection)
        {
            servicesCollection.AddTransient<ITreeViewInterop, TreeViewInterop>();

            return servicesCollection;
        }
    }
}
