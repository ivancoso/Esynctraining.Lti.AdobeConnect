using System;
using System.Collections.Specialized;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace EdugameCloud.ACEvents.Web
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
                
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations();

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

           
        }
    }
}