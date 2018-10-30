using System;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using EdugameCloud.Lti.API.Schoology;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Core.Providers;
using Esynctraining.Extensions;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.BlackBoard;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Lti.Zoom.Api.Host.BackgroundServices;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Api.Host.Swagger;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.HostedServices;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Mail;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;
using Esynctraining.Mail.SmtpClient.MailKit;
using Esynctraining.Mail.TemplateTransform.RazorLight;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Esynctraining.Lti.Zoom.Api.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IHostingEnvironment HostingEnvironment { get; }


        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services
                .AddMvcCore(setup =>
                {
                    setup.InputFormatters.Insert(0, new JilInputFormatter());
                    setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(BaseApiController).Assembly)
                .AddControllersAsServices()
                .AddApiExplorer()
                .AddDataAnnotations();
            if (bool.TryParse(Configuration["UseRedis"], out bool useRedis) && !useRedis)
            {
                services.AddDistributedMemoryCache();
                services.AddHostedService<UserCacheHostedService>(); //don't need if using redis - cache is refreshed by Zoom.Host
            }
            else
            {
                services
                    .AddDistributedRedisCache(options =>
                    {
                        options.Configuration = Configuration.GetConnectionString("CacheRedis");
                    });
            }
            services
                .AddCors();
            var configurationSection = Configuration.GetSection("AppSettings");
            var settings = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            services
                .AddSingleton(new ApplicationSettingsProvider(settings));
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddLtiSwagger(HostingEnvironment);
            }

            services.AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();

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
            services.AddTransient<ILmsLicenseService, LmsLicenseInternalApiService>();
            services.AddTransient<UserSessionService, UserSessionService>();
            services.AddSingleton<IJsonSerializer, JilSerializer>();
            services.AddSingleton<IJsonDeserializer, JilSerializer>();
            services.AddTransient<IEGCEnabledCanvasAPI, EGCEnabledCanvasAPI>();
            services.AddTransient<IAgilixBuzzApi, DlapAPI>();
            services.AddTransient<ISchoologyRestApiClient, SchoologyRestApiClient>();
            services.AddTransient<IBlackBoardApi, SoapBlackBoardApi>();
            services.AddTransient<CanvasLmsUserService, CanvasLmsUserService>();
            services.AddTransient<AgilixBuzzLmsUserService, AgilixBuzzLmsUserService>();
            services.AddTransient<SchoologyLmsUserService, SchoologyLmsUserService>();
            services.AddTransient<BlackboardLmsUserService, BlackboardLmsUserService>();
            services.AddTransient<KalturaService, KalturaService>();
            services.AddTransient<ZoomUserService, ZoomUserService>();
            services.AddTransient<ZoomRecordingService, ZoomRecordingService>();
            services.AddTransient<ExternalStorageService, ExternalStorageService>();
            services.AddTransient<ZoomReportService, ZoomReportService>();
            services.AddTransient<ZoomMeetingService, ZoomMeetingService>();
            services.AddTransient<ZoomMeetingApiService, ZoomMeetingApiService>();
            services.AddTransient<IMeetingSessionService, MeetingSessionService>();
            services.AddTransient<ZoomOfficeHoursService, ZoomOfficeHoursService>();
            services.AddSingleton<ILtiTokenAccessor, LtiTokenAccessor>();
            services.AddSingleton<LmsUserServiceFactory, LmsUserServiceFactory>();
            services.AddScoped<ZoomLicenseAccessor, ZoomLicenseAccessor>();
            services.AddScoped<IZoomOptionsAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ILmsLicenseAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ZoomApiWrapper, ZoomApiWrapper>();
            //services.Configure<TemplateSettings>(Configuration.GetSection("MailTemplateSettings")).AddSingleton<ITemplateSettings>(sp => sp.GetService<IOptions<TemplateSettings>>().Value);
            //services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings")).AddSingleton<ISmtpSettings>(sp => sp.GetService<IOptions<SmtpSettings>>().Value);
            services.ConfigureSingleton<ISmtpSettings, SmtpSettings>(Configuration.GetSection("SmtpSettings"), x => x)
                .ConfigureSingleton<ITemplateSettings, TemplateSettings>(Configuration.GetSection("MailTemplateSettings"), x => x)
                .AddScoped<ITemplateTransformer, ViewRenderService>()
                .AddScoped<ISmtpClient, MailKitSmtpClient>()
                .AddScoped<INotificationService, NotificationService>();

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<UserCacheUpdater, UserCacheUpdater>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            ServicePointManager.DefaultConnectionLimit = int.Parse(Configuration["AppSettings:ConnectionBatchSize"]);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //}

            var origins = Configuration["AppSettings:CorsOrigin"].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            app.UseCors(builder =>
                builder
                .WithOrigins(origins)
                .WithMethods("POST", "GET", "DELETE", "PUT")
                .WithHeaders("Authorization", "X-Requested-With", "Content-Type", "Accept", "Origin")
                .SetPreflightMaxAge(TimeSpan.FromDays(1)));

            app.UseMvc(cfg => 
            {
            });
            if (env.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/v1/swagger/v1/swagger.json", "eSyncTraining LTI Zoom API V1");
                });
            }
        }

    }

}
