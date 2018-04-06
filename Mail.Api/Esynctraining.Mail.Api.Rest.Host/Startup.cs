using System;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Extensions;
using Esynctraining.Mail;
using Esynctraining.Mail.Api.Rest.Host;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;
using Esynctraining.Mail.SmtpClient.MailKit;
using Esynctraining.Mail.TemplateTransform.RazorLight2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace AnonymousChat.WebApi.Host
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IConfiguration Configuration { get; }


        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SmtpClientCheck>();

            services.AddHealthChecks(checks =>
            {
                checks
                    .AddCheck<SmtpClientCheck> ("SMTP Check");
            });
            
            services
                .AddMvcCore(setup =>
                {
                    while (setup.InputFormatters.Count > 0)
                        setup.InputFormatters.RemoveAt(0);
                    while (setup.OutputFormatters.Count > 0)
                        setup.OutputFormatters.RemoveAt(0);

                    setup.InputFormatters.Insert(0, new JilInputFormatter());
                    setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                //.AddApplicationPart(typeof(BaseController).Assembly)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations()
                //.AddAuthorization() // Note - this is on the IMvcBuilder, not the service collection
                ;

            services
                .AddSingleton(LoggerFactory)
                .AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();

            var basicLogger = LoggerFactory.CreateLogger("Basic");
            services
                .AddSingleton(basicLogger)
                .ConfigureSingleton<ISmtpSettings, SmtpSettings>(Configuration.GetSection("SmtpSettings"), x => x)
                .ConfigureSingleton<ITemplateSettings, TemplateSettings>(Configuration.GetSection("MailTemplateSettings"), x => x)
                .ConfigureSingleton<INotificationsSettings, NotificationsSettings>(Configuration.GetSection("Mailer"), x => x)
                .AddSingleton<ITemplateTransformer, ViewRenderService>()
                .AddSingleton<ISmtpClient, MailKitSmtpClient>()
                ;

            if (!HostingEnvironment.IsProduction())
            {
                AddSwagger(services, HostingEnvironment);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            if (!HostingEnvironment.IsProduction())
            {
                app.UseSwagger(c =>
                {
                });

                bool isDebug = false;
#if DEBUG
                isDebug = true;
#endif
                app.UseSwaggerUI(c =>
                {
                    //(env.IsDevelopment() ? string.Empty : "/api")
                    var swaggerEndpointUrl = isDebug ? "/swagger/v1/swagger.json" : "/v1/swagger/v1/swagger.json";
                    c.SwaggerEndpoint(swaggerEndpointUrl, "API V1");
                });
            }

        }

        private static IServiceCollection AddSwagger(IServiceCollection services, IHostingEnvironment env)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
                //c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                //c.IncludeXmlComments(string.Format(@"{0}\Boss.Reporting.Api.Host.xml", env.WebRootPath));
                c.DescribeAllEnumsAsStrings();

                c.MapType<DateTime?>(() =>
                    new Schema
                    {
                        Type = "integer",
                        Format = "int64",
                        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        Description = "MillisecondsSinceUnixEpoch",
                        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        //Pattern = "pattern",
                    }
                );

                c.MapType<DateTime>(() =>
                    new Schema
                    {
                        Type = "integer",
                        Format = "int64",
                        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        Description = "MillisecondsSinceUnixEpoch",
                        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        //Pattern = "pattern",
                    }
                );

            });
            return services;
        }


    }

}
