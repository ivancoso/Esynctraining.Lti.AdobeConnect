using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using EdugameCloud.Lti.Api.Host.Swagger;
using EdugameCloud.Persistence;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Extensions;
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
                    //while (setup.InputFormatters.Count > 0)
                    //    setup.InputFormatters.RemoveAt(0);
                    //while (setup.OutputFormatters.Count > 0)
                    //    setup.OutputFormatters.RemoveAt(0);

                    setup.InputFormatters.Insert(0, new JilInputFormatter());
                    setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    //setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(Controllers.BaseApiController).Assembly)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
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

            services
                .AddCors()
                .AddMemoryCache();

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
                
                if (!HostingEnvironment.IsDevelopment())
                    c.DocumentFilter<HideNonApiFilter>();

                c.SchemaFilter<ApplyCustomSchemaFilters>();

                c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.Api.xml", HostingEnvironment.WebRootPath));
                c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.Core.xml", HostingEnvironment.WebRootPath));
                c.IncludeXmlComments(string.Format(@"{0}\Esynctraining.AdobeConnect.Api.xml", HostingEnvironment.WebRootPath));
                c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.xml", HostingEnvironment.WebRootPath));
                
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization header using the Bearer scheme. " +
                    "<br/> Example: \"Authorization: ltiapi {consumerKey};{courseId}\""
                    + "<br/> {courseId} - LMS course Id."
                    + "For Sakai {courseId} - is GetHashCode() result for course context_id value.",
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

            // https://docs.microsoft.com/en-us/aspnet/core/security/cors
            //app.Use(next =>
            //{
            //    return ctx =>
            //    {
            //        // NOTE: skip Swagger related requests
            //        if (ctx.Request.Method == "OPTIONS" || ctx.Request.Method == "POST")
            //        {
            //            // TODO: GC performances
            //            var origins = Configuration["AppSettings:CorsOrigin"].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            //            ctx.Response.Headers.Add("Access-Control-Allow-Origin", origins);
            //            ctx.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Authorization, X-Requested-With, Content-Type, Accept, Origin" });
            //            ctx.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST" });
            //            ctx.Response.Headers.Add("Access-Control-Max-Age", new[] { "86400" }); // 1day

            //            if (ctx.Request.Method == "OPTIONS")
            //            {
            //                return Task.FromResult(0);
            //            }
            //        }

            //        return next(ctx);
            //    };
            //});

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
                .WithHeaders(new[] { "Authorization, X-Requested-With, Content-Type, Accept, Origin" })
                .SetPreflightMaxAge(TimeSpan.FromDays(1)));


            app.UseMvc(cfg => 
            {
            });

            //app.UseCors("ALL");

            app.UseSwagger(c => 
            {
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/lti-api/swagger/v1/swagger.json", "eSyncTraining LTI API V1");
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
            // container.Install(new LtiMvcWindsorInstaller());
            container.Install(new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }
    }

}
