using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreeDocs.Auth.Entities;
using MongoDB.Bson;
using MongoDbGenericRepository;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Extensions;
using TreeDocs.Auth.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TreeDocs.Auth.IdentityExtensions;

namespace TreeDocs.Auth
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment _env;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                //per user config that is not committed to repo, use this to override settings (e.g. connection string) based on your local environment.
                .AddJsonFile($"appsettings.local.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoSettings = Configuration.GetSection(nameof(MongoDbSettings));
            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            services.AddSingleton<MongoDbSettings>(settings);

            Action<IdentityOptions> IdentityOptionsAction = (options) =>
                            {
                                options.Password.RequireDigit = false;
                                options.Password.RequiredLength = 6;
                                options.Password.RequireNonAlphanumeric = false;
                                options.Password.RequireUppercase = false;
                                options.Password.RequireLowercase = false;

                                // Lockout settings
                                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                                options.Lockout.MaxFailedAccessAttempts = 10;

                                // ApplicationUser settings
                                options.User.RequireUniqueEmail = true;
                                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
                            };

            services.AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsAction)
                    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(settings.ConnectionString, settings.DatabaseName)
                    .AddSignInManager()
                    .AddDefaultTokenProviders();

            services.SetAppIdentityErrorDescriber(); // my identity error describer to support errors localization. Must be put after AddIdentity

            services.AddRazorPages();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
