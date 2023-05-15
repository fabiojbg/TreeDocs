using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Apps.Sdk.DependencyInjection;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TreeDocs.Service.Authorization;
using TreeDocs.Service.Services;
using Microsoft.IdentityModel.Logging;
using Apps.Sdk.Middlewares;
using Microsoft.Net.Http.Headers;
using App.Sdk.DependencyInjection;
using Domain.Shared;

namespace TreeDocs.Service
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            // In ASP.NET Core 3.0 `env` will be an IWebHostEnvironment, not IHostingEnvironment.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .AddControllersAsServices()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.IgnoreNullValues = true; // do not serialize null properties
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // make enums be serialized as String rather than Int
                        options.JsonSerializerOptions.PropertyNamingPolicy = null; // avoid the use of camel case in serialized prorperties
                    });

            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("TreeDocsServicePolicy",
                                  builder => builder.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                                   );
            });

            var secretKey = TokenService.GetSecretKey(Configuration);
            var encryptionKey = TokenService.GetEncryptionKey(Configuration);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    TokenDecryptionKey = encryptionKey==null ? null : new SymmetricSecurityKey(encryptionKey)
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string authorization = context.Request.Headers[HeaderNames.Authorization];
                        if (authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            context.Token = TokenService.DecodeToken(authorization.Substring("Bearer ".Length).Trim()); //Decode my encoded Token
                        }
                        return Task.CompletedTask;
                    }
                    
                };
            });
            
            Application.DependencyInjection.AddApplication(services);

            IdentityModelEventSource.ShowPII = Configuration.GetValue<bool>("General:ShowPII", false);
        }

        public void ConfigureContainer(ContainerBuilder builder) // this is called by autofac
        {
            var sdkContainerBuilder = new AutofacContainerBuilder(builder);

            sdkContainerBuilder.RegisterSingleton<ISdkContainer>(c =>new AutofacContainer(this.AutofacContainer));

            sdkContainerBuilder.RegisterScoped<IAppUserService, AppUserService>();

            Apps.Sdk.Web.DependencyInjection.RegisterServices(sdkContainerBuilder, Configuration);

            Application.DependencyInjection.RegisterDependencies(sdkContainerBuilder, Configuration);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            SdkDI.SetGlobalResolver(this.AutofacContainer.Resolve<ISdkContainer>());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }
            app.UseSdkExceptionInterceptor();
            app.UseSdkRequestLanguageDetection();

            app.UseCors("TreeDocsServicePolicy");

            //app.UseHttpsRedirection(); not recommended for Apis
            app.UseStaticFiles();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseSerilogRequestLogging();
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

            });

        }
    }
}
