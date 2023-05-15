using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace TreeDocs.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
                                                                               .ReadFrom.Configuration(hostingContext.Configuration) // configuration in AppSettings
                                                                               //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                                                                               //.MinimumLevel.Information()
                                                                               .Enrich.FromLogContext()
                                                                               //.WriteTo.Console()
                                                                               //.WriteTo.Seq("http://localhost:5341")
                                                                            )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
