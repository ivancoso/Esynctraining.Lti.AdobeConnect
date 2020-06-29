using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.API.Schoology;
using Esynctraining.Lti.Lms.Common.HttpClient;
using Esynctraining.Lti.Lms.Desire2Learn;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Lti.Lms.Sakai;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Lti.Zoom.Api.Host.BackgroundServices;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Api.Host.Swagger;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Mail;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;
using Esynctraining.Mail.SmtpClient.MailKit;
using Esynctraining.Mail.TemplateTransform.RazorLight;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Logging;
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
                    setup.Filters.Add(new Esynctraining.Lti.Zoom.Api.Host.Filters.ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(BaseApiController).Assembly)
                .AddControllersAsServices()
                .AddApiExplorer()
                .AddDataAnnotations();

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

            //if (bool.TryParse(settings["UseRedis"], out bool useRedis) && !useRedis)
            //{
            //    services.AddDistributedMemoryCache();
            //}
            //else
            //{
            //    services
            //        .AddDistributedRedisCache(options =>
            //        {
            //            options.Configuration = Configuration.GetConnectionString("CacheRedis");
            //        });
            //}

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

            services = RegisterLtiServices(services);

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
            services.AddScoped<ILmsLicenseAccessor, LicenseAccessor>();

            //services.Configure<TemplateSettings>(Configuration.GetSection("MailTemplateSettings")).AddSingleton<ITemplateSettings>(sp => sp.GetService<IOptions<TemplateSettings>>().Value);
            //services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings")).AddSingleton<ISmtpSettings>(sp => sp.GetService<IOptions<SmtpSettings>>().Value);
            services.ConfigureSingleton<ISmtpSettings, SmtpSettings>(Configuration.GetSection("SmtpSettings"), x => x)
                .ConfigureSingleton<ITemplateSettings, TemplateSettings>(Configuration.GetSection("MailTemplateSettings"), x => x)
                .AddScoped<ITemplateTransformer, ViewRenderService>()
                .AddScoped<ISmtpClient, MailKitSmtpClient>()
                .AddScoped<INotificationService, NotificationService>();

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            //todo: uncomment JWT for previous workflow
            //services.AddScoped<IZoomApiJwtOptionsAccessor, ZoomJwtOptionsFromLicenseAccessor>();
            //services.AddScoped<IZoomAuthParamsAccessor, ZoomJwtAuthParamsAccessor>();
            services.ConfigureSingleton<ZoomOAuthConfig, ZoomOAuthConfig>(Configuration.GetSection("ZoomOAuthConfig"), x => x);
            services.AddScoped<IZoomOAuthOptionsAccessor, ZoomOAuthOptionsFromLicenseAccessor>();
            services.AddScoped<IZoomAuthParamsAccessor, ZoomOAuthParamsAccessor>();
            services.AddScoped<ZoomApiWrapper, ZoomApiWrapper>();
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

        private IServiceCollection RegisterLtiServices(IServiceCollection services)
        {
            services = RegisterHttpClients(services);

            services.AddTransient<IMoodleApi, MoodleApi>();
            services.AddTransient<MoodleLmsUserService, MoodleLmsUserService>();

            services.AddTransient<IDesire2LearnApiService, Desire2LearnApiService>();
            services.AddTransient<Desire2LearnLmsUserService, Desire2LearnLmsUserService>();

            services.AddTransient<IEGCEnabledCanvasAPI, EGCEnabledCanvasAPI>();
            services.AddTransient<CanvasLmsUserService, CanvasLmsUserService>();
            services.AddTransient<CanvasCalendarEventService, CanvasCalendarEventService>();

            services.AddTransient<IAgilixBuzzApi, DlapAPI>();
            services.AddTransient<AgilixBuzzLmsUserService, AgilixBuzzLmsUserService>();

            services.AddTransient<IBlackBoardApi, SoapBlackBoardApi>();
            services.AddTransient<BlackboardLmsUserService, BlackboardLmsUserService>();

            services.AddTransient<SakaiLmsUserService, SakaiLmsUserService>();

            services.AddTransient<SchoologyLmsUserService, SchoologyLmsUserService>();

            services.AddTransient<LmsCalendarEventServiceFactory, LmsCalendarEventServiceFactory>();
            services.AddTransient<LmsUserServiceFactory, LmsUserServiceFactory>();
            return services;
        }

        private IServiceCollection RegisterHttpClients(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<LoggingHttpMessageHandler>((container) =>
            {
                var logger = container.GetRequiredService<ILogger<LoggingHttpMessageHandler>>();
                return new LoggingHttpMessageHandler(logger);
            });
            serviceCollection.AddHttpClient();
            serviceCollection.AddHttpClient(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientName, c =>
            {
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientTimeout);
            });
            serviceCollection.AddHttpClient<BuzzApiClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AppSettings:AgilixBuzzApiUrl"]);
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.BuzzApiClientTimeout);
                c.DefaultRequestHeaders.Add("User-Agent", "eSync");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    CookieContainer = new System.Net.CookieContainer()
                };
            }).AddHttpMessageHandler<LoggingHttpMessageHandler>(); //enable Trace level for logging headers

            serviceCollection.AddHttpClient<LTI2Api>();

            serviceCollection.AddHttpClient<ISchoologyRestApiClient, SchoologyRestApiClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AppSettings:SchoologyApiUrl"]);
            });

            serviceCollection.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HttpMsLoggingFilter>());

            return serviceCollection;
        }

    }

}
