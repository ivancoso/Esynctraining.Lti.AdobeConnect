using System;
using System.Net;
using System.Net.Http;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor.MsDependencyInjection;
using EdugameCloud.Lti.Api.Host.Swagger;
using EdugameCloud.Lti.Sakai;
using EdugameCloud.Lti.Telephony;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.HttpClient;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.Sakai;
using Esynctraining.Lti.Lms.Common.API.Schoology;
using Esynctraining.Lti.Lms.Sakai;
using Esynctraining.Lti.Lms.Schoology;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace EdugameCloud.Lti.Api.Host
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services.Configure<MeetingOneSettings>(Configuration.GetSection("MeetingOneSettings"))
                .AddSingleton(sp => sp.GetService<IOptionsSnapshot<MeetingOneSettings>>().Value);

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
                    setup.Filters.Add(new EdugameCloud.Lti.Api.Core.Filters.ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(Controllers.BaseApiController).Assembly)
                .AddControllersAsServices()
                //.AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations();

            services
                .AddCors()
                .AddMemoryCache();

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services = RegisterHttpClients(services);
            var container = DIConfig.ConfigureWindsor(Configuration);
            services.AddRequestScopingMiddleware(container.BeginScope);

            services.AddLtiSwagger(HostingEnvironment);

            //services.AddOptions();

            return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        private IServiceCollection RegisterHttpClients(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<DebugHttpLoggingHandler>();
            serviceCollection.AddHttpClient();
            serviceCollection.AddHttpClient(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientName, c =>
            {
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientTimeout);
            });
            serviceCollection.AddHttpClient<BuzzApiClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AppSettings:AgilixBuzzApiUrl"]);
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.BuzzApiClientTimeout);
                c.DefaultRequestHeaders.Add("User-Agent", "EduGameCloud");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    CookieContainer = new System.Net.CookieContainer()
                };
            }).AddHttpMessageHandler<DebugHttpLoggingHandler>();

            serviceCollection.AddHttpClient<LTI2Api>();
            serviceCollection.AddHttpClient<IEGCEnabledSakaiApi, SakaiApi>();

            serviceCollection.AddHttpClient<ISchoologyRestApiClient, SchoologyRestApiClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AppSettings:SchoologyApiUrl"]);
            });

            serviceCollection.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, CustomLoggingFilter>());

            return serviceCollection;
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

            //app.UseExceptionHandler(
            //    builder =>
            //    {
            //        builder.Run(
            //        async context =>
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.OK;
            //            var ex = context.Features.Get<IExceptionHandlerFeature>();
            //            if (ex != null)
            //            {
            //            }
            //        });
            //    });
            var origins = Configuration["AppSettings:CorsOrigin"].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            app.UseCors(builder =>
                builder
                .WithOrigins(origins)
                .WithMethods("POST")
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
                c.SwaggerEndpoint("/lti-api/swagger/v1/swagger.json", "eSyncTraining LTI API V1");
            });
        }

    }

}
