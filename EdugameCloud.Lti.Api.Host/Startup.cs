using System;
using EdugameCloud.Lti.Api.Host.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Castle.Windsor;
using Esynctraining.Windsor;
using EdugameCloud.Persistence;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using FluentValidation;
using Castle.Windsor.MsDependencyInjection;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;
using Castle.MicroKernel.Lifestyle;
using System.Collections.Specialized;

namespace EdugameCloud.Lti.Api.Host
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services
                .AddMvcCore(setup =>
                {
                    //setup.
                })
                .AddApplicationPart(typeof(EdugameCloud.Lti.Api.Controllers.BaseApiController).Assembly)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer();

            var container = new WindsorContainer();

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.Scoped());
            services.AddRequestScopingMiddleware(container.BeginScope);

            WindsorIoC.Initialize(container);
            container.RegisterComponents();

            var configurationSection = Configuration.GetSection("AppSettings");
            var collection = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                collection.Add(appSetting.Key, appSetting.Value);
            }

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", collection))
                .LifeStyle.Singleton);

            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            RegisterLtiComponents(container);

            //.AddJsonOptions(options =>
            // {
            //     var settings = options.SerializerSettings;
            //     settings.Formatting = Formatting.Indented;
            //     settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            //     settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            //     settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //     settings.NullValueHandling = NullValueHandling.Ignore;
            // });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "eSyncTraining LTI API", Version = "v1" });
                //c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                c.DocumentFilter<HideNonApiFilter>();

                // c.IncludeXmlComments(String.Format(@"{0}\SO.WebServices.XML", AppDomain.CurrentDomain.BaseDirectory)); // Use xml comments for Swagger documentation

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "Authorization header using the Bearer scheme. Example: \"Authorization: lti {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

            });

            //services.AddOptions();

            return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next =>
            {
                return ctx =>
                {
                    ctx.Response.Headers.Remove("Server");
                    return next(ctx);
                };
            });


            app.UseMvc();

            //app.UseCors("ALL");

            app.UseSwagger(c => 
            {
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "eSyncTraining LTI API V1");
            });
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.BrainHoney/EdugameCloud.Lti.BrainHoney.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());
            container.Install(new LtiMvcWindsorInstaller());
            container.Install(new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }
    }

}
