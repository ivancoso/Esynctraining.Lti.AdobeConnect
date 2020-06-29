using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using EdugameClaud.Lti.SearchApi.Host.Models;
using EdugameCloud.Lti.SearchApi.Host.Models;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Core.Providers;
using Esynctraining.Json.Jil;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace EdugameClaud.Lti.SearchApi.Host
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services
                    .AddMvcCore(setup =>
                    {
                        setup.InputFormatters.Insert(0, new JilInputFormatter());
                        setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                        setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                        setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                    })
                    .AddControllersAsServices()
                    .AddApiExplorer()
                    .AddDataAnnotations();

            services.AddSingleton<IJsonDeserializer, JilSerializer>();
            services.AddCors();

            var configurationSection = Configuration.GetSection("AppSettings");
            var settings = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            services
                .AddSingleton(new ApplicationSettingsProvider(settings));

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            //if (HostingEnvironment.IsDevelopment())
            //{
            //    services.AddLtiSwagger(HostingEnvironment);
            //}

            services.AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();
            var connectionString = Configuration.GetConnectionString("Db");
            services.AddDbContext<EduGameCloudContext>(options => options.UseSqlServer(connectionString));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseHsts();
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
                //app.UseSwagger(c =>
                //{
                //});

                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/v1/swagger/v1/swagger.json", "eSyncTraining LTI Zoom API V1");
                //});
            }
        }
    }
}
