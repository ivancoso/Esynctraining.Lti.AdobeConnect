using System;
using System.Collections.Specialized;
using System.Reflection;
using EdugameCloud.Lti.API.Schoology;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Core.Providers;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.BlackBoard;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Lti.Zoom.Api;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Core;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Lti.Zoom.Routes;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Esynctraining.Lti.Zoom.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = Assembly.Load(new AssemblyName("Esynctraining.Lti.Zoom"));
            services
                .AddMvc()
                .AddApplicationPart(controllerAssembly)
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services
                .AddDistributedRedisCache(options =>
                {
                    options.Configuration = Configuration.GetConnectionString("CacheRedis");
                });
            services
                .AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();

            var settings = new NameValueCollection();
            var version = typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            settings.Add("Version", version);
            var configurationSection = Configuration.GetSection("AppSettings");
            
            foreach (var appSetting in configurationSection.GetChildren())
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            services
                .AddSingleton(new ApplicationSettingsProvider(settings));

            services
                .AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor,
                    Microsoft.AspNetCore.Http.HttpContextAccessor>();

            var migrationsAssembly = typeof(ZoomDbContext).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("ZoomDb");
            services.AddDbContext<ZoomDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));
            services.AddDbContext<ZoomDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ZoomDb")));
            services.AddTransient<ILmsLicenseService, LmsLicenseInternalApiService>();
            services.AddTransient<UserSessionService, UserSessionService>();
            services.AddSingleton<JilSerializer, JilSerializer>();
            services.AddSingleton<IJsonSerializer, JilSerializer>(s => s.GetService<JilSerializer>());
            services.AddSingleton<IJsonDeserializer, JilSerializer>(s => s.GetService<JilSerializer>());
            services.AddTransient<IEGCEnabledCanvasAPI, EGCEnabledCanvasAPI>();
            services.AddTransient<LmsUserServiceBase, CanvasLmsUserService>();
            services.AddTransient<IAgilixBuzzApi, DlapAPI>();
            services.AddTransient<ISchoologyRestApiClient, SchoologyRestApiClient>();
            services.AddTransient<IBlackBoardApi, SoapBlackBoardApi>();
            services.AddTransient<CanvasLmsUserService, CanvasLmsUserService>();
            services.AddTransient<AgilixBuzzLmsUserService, AgilixBuzzLmsUserService>();
            services.AddTransient<SchoologyLmsUserService, SchoologyLmsUserService>();
            services.AddTransient<BlackboardLmsUserService, BlackboardLmsUserService>();
            services.AddSingleton<LmsUserServiceFactory, LmsUserServiceFactory>();

            services.AddTransient<ZoomUserService, ZoomUserService>();
            services.AddTransient<ZoomReportService, ZoomReportService>();
            services.AddTransient<ZoomMeetingService, ZoomMeetingService>();
            services.AddScoped<ZoomMeetingApiService, ZoomMeetingApiService>();
            services.AddTransient<ZoomOfficeHoursService, ZoomOfficeHoursService>();

            services.AddSingleton<ILtiTokenAccessor, QueryStringTokenAccessor>();
            services.AddScoped<ZoomLicenseAccessor, ZoomLicenseAccessor>();
            services.AddScoped<IZoomOptionsAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ILmsLicenseAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ZoomApiWrapper, ZoomApiWrapper>();
            services.AddScoped<INotificationService, EmptyNotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            {
                app.UseExceptionHandler("/Error/Index");
                app.UseHsts();
            }

            var origins = Configuration.GetSection("AllowedHosts").Value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            app.UseCors(builder =>
                builder
                    .WithOrigins(origins)
                    // TODO: use if required .WithMethods("POST")
                    .AllowAnyMethod()
                    .WithHeaders("Authorization", "X-Requested-With", "Content-Type", "Accept", "Origin")
                    .SetPreflightMaxAge(TimeSpan.FromDays(1)));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(LtiRoutes.AppendTo);
        }
    }
}