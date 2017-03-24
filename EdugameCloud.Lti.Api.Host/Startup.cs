using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Host.Formatters;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;
using Esynctraining.Windsor;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace EdugameCloud.Lti.Api.Host
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IHostingEnvironment HostingEnvironment { get; }


        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;

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
                    setup.InputFormatters.Insert(0, new JilInputFormatter());
                    setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    setup.Filters.Add(new CheckModelForNullAttribute());
                    //setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(Controllers.BaseApiController).Assembly)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
                //.AddCors()
                .AddDataAnnotations();
                //.AddJsonOptions(options =>
                // {
                //     var settings = options.SerializerSettings;
                //     settings.Formatting = Formatting.Indented;
                //     settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                //     settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //     settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //     settings.NullValueHandling = NullValueHandling.Ignore;
                // });


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

            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "eSyncTraining LTI API", Version = "v1" });
                //c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                // HACK: umcomment for prod.
                //if (HostingEnvironment.IsProduction())
                //    c.DocumentFilter<HideNonApiFilter>();
                
                //c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.Api.xml", HostingEnvironment.WebRootPath)); 
                
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
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
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug()
                .AddFile(Configuration.GetSection("Logging"))
                .AddFile(Configuration.GetSection("Logging_Serilog_Errors"));


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next =>
            {
                return ctx =>
                {
                    ctx.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    ctx.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Authorization, X-Requested-With, Content-Type, Accept, Origin" });
                    ctx.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST" });
                    ctx.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });

                    if (ctx.Request.Method == "OPTIONS")
                    {
                        return Task.FromResult(0);
                    }

                    return next(ctx);
                };
            });

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
            app.UseMvc(cfg => 
            {
            });

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
