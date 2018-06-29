using System;
using System.Collections.Specialized;
using System.Net;
using EdugameCloud.Lti.Canvas;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Core.Providers;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Host.Swagger;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
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

            services
                .AddCors();
            //.AddMemoryCache();
            var configurationSection = Configuration.GetSection("AppSettings");
            var settings = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            services
                .AddSingleton(new ApplicationSettingsProvider(settings));
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            services.AddLtiSwagger(HostingEnvironment);

            services.AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();
            services.AddDbContext<ZoomDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ZoomDb")));
            services.AddTransient<ILmsLicenseService, LmsLicenseDbService>();
            services.AddTransient<UserSessionService, UserSessionService>();
            services.AddSingleton<IJsonSerializer, JilSerializer>();
            services.AddSingleton<IJsonDeserializer, JilSerializer>();
            services.AddTransient<IEGCEnabledCanvasAPI, EGCEnabledCanvasAPI>();
            services.AddTransient<LmsUserServiceBase, CanvasLmsUserService>();
            services.AddTransient<ZoomUserService, ZoomUserService>();
            services.AddTransient<ZoomRecordingService, ZoomRecordingService>();
            services.AddTransient<ZoomReportService, ZoomReportService>();
            services.AddTransient<ZoomMeetingService, ZoomMeetingService>();
            services.AddTransient<ZoomOfficeHoursService, ZoomOfficeHoursService>();
            services.AddSingleton<ILtiTokenAccessor, LtiTokenAccessor>();
            services.AddScoped<ZoomLicenseAccessor, ZoomLicenseAccessor>();
            services.AddScoped<IZoomOptionsAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ILmsLicenseAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ZoomApiWrapper, ZoomApiWrapper>();
            //services.AddOptions();

            //return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            ServicePointManager.DefaultConnectionLimit = int.Parse(Configuration["AppSettings:ConnectionBatchSize"]);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            //log
                        }
                    });
                });
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

            app.UseSwagger(c => 
            {
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/lti-zoom-api/swagger/v1/swagger.json", "eSyncTraining LTI Zoom API V1");
            });
        }

    }

}
