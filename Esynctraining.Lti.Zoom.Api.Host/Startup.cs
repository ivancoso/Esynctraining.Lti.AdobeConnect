using System;
using System.Net;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Api.Host.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Esynctraining.Lti.Zoom.Api.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory { get; }

        public IHostingEnvironment HostingEnvironment { get; }


        public Startup(IHostingEnvironment env, IConfiguration configuration, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
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

            //services.Configure<MeetingOneSettings>(Configuration.GetSection("MeetingOneSettings"))
            //    .AddSingleton(sp => sp.GetService<IOptionsSnapshot<MeetingOneSettings>>().Value);

            services
                .AddMvcCore(setup =>
                {
                    //while (setup.InputFormatters.Count > 0)
                    //    setup.InputFormatters.RemoveAt(0);
                    //while (setup.OutputFormatters.Count > 0)
                    //    setup.OutputFormatters.RemoveAt(0);

                    setup.InputFormatters.Insert(0, new JilInputFormatter());
                    setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(BaseApiController).Assembly)
                .AddControllersAsServices()
                //.AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations();

            services
                .AddCors()
                .AddMemoryCache();

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            //var container = DIConfig.ConfigureWindsor(Configuration);
            //services.AddRequestScopingMiddleware(container.BeginScope);

            services.AddLtiSwagger(HostingEnvironment);

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
                //app.UseDeveloperExceptionPage();
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
