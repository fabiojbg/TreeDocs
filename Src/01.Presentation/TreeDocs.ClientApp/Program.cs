using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Apps.Blazor.Components;
using TreeDocs.ClientApp.Services;
using Blazored.Toast;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TreeDocs.ClientApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // register syncfusion licence 19.1.0.63
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDU4NjQ0QDMxMzkyZTMxMmUzMGFXTWF5L0E4U3B2L3lEZkhrMFVoR3hXUVRWVkJZMlZpQXViWUhVZTc4Mm89");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddAppsBlazorComponents();
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);
            builder.Services.AddScoped<INotifier, BlazoredToastNotifier>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            //builder.Services.AddTransient(sp => new HttpClient(new DefaultMessageHandler(new HttpClientHandler())
            //{
            //    DefaultBrowserRequestCache = null,
            //    DefaultBrowserRequestCredentials = null,
            //    DefaultBrowserRequestMode = BrowserRequestMode.NoCors,
            //})
            //{
            //    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
            //});
            
            builder.Services.AddScoped<AppState, AppState>();
            
            await builder.Build().RunAsync();
        }
    }
}
