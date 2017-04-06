using System;
using System.Collections.Specialized;
using Castle.Core.Logging;
using CompanyAcDomainsNamespace;
using CompanyEventsServiceNamespace;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EdugameCloud.Core.Logging;
using EmailServiceNamespace;
using Esynctraining.AdobeConnect;
using FileServiceNamespace;
using LookupServiceNamespace;
using QuizResultServiceNamespace;
using QuizServiceNamespace;
using Esynctraining.AspNetCore.Filters;
//using Logger = EdugameCloud.Core.Logging.Logger;
using ILogger = Esynctraining.Core.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services
                .AddMvc(setup =>
                {
                    //while (setup.InputFormatters.Count > 0)
                    //    setup.InputFormatters.RemoveAt(0);
                    //while (setup.OutputFormatters.Count > 0)
                    //    setup.OutputFormatters.RemoveAt(0);

                    //setup.InputFormatters.Insert(0, new JilInputFormatter());
                    //setup.OutputFormatters.Insert(0, new JilOutputFormatter());

                    //setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })

                .AddControllersAsServices();
                //.AddJsonFormatters()
                //.AddApiExplorer()
                //.AddDataAnnotations();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services
                //.AddCors()
                //.AddMemoryCache();

            //var container = new WindsorContainer();

            //container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.Scoped());
            //services.AddRequestScopingMiddleware(container.BeginScope);

            //WindsorIoC.Initialize(container);
            //container.RegisterComponents();

            //var configurationSection = Configuration.GetSection("AppSettings");
            //var collection = new NameValueCollection();
            //foreach (var appSetting in configurationSection.GetChildren())
            //{
            //    collection.Add(appSetting.Key, appSetting.Value);
            //}


            //var fact = new ConsoleFactory();
            //services.AddSingleton<Castle.Core.Logging.ILoggerFactory>(fact);
            //services.AddSingleton<Castle.Core.Logging.ILoggerFactory, ConsoleFactory>();
            services.AddSingleton<ILogger, FakeLogger>();

            services.AddScoped<IEmailService>(provider =>
            {
                var emailServiceUrl = Configuration["WebServiceReferences:EmailService"];
                var client = new EmailServiceClient(EmailServiceClient.EndpointConfiguration.BasicHttpBinding_IEmailService, emailServiceUrl);
                return client;
            });
            services.AddScoped<ILookupService>(provider =>
            {
                var lookupServiceUrl = Configuration["WebServiceReferences:LookupService"];
                var client = new LookupServiceClient(LookupServiceClient.EndpointConfiguration.BasicHttpBinding_ILookupService, lookupServiceUrl);
                return client;
            });
            services.AddScoped<ICompanyEventsService>(provider =>
            {
                var companyEventsServiceUrl = Configuration["WebServiceReferences:CompanyEventsService"];
                var client = new CompanyEventsServiceClient(CompanyEventsServiceClient.EndpointConfiguration.BasicHttpBinding_ICompanyEventsService, companyEventsServiceUrl);
                return client;
            });
            services.AddScoped<ICompanyAcDomainsService>(provider =>
            {
                var companyAcDomainsServiceUrl = Configuration["WebServiceReferences:CompanyAcDomainsService"];
                var client = new CompanyAcDomainsServiceClient(CompanyAcDomainsServiceClient.EndpointConfiguration.BasicHttpBinding_ICompanyAcDomainsService, companyAcDomainsServiceUrl);
                return client;
            });
            //services.AddScoped<ILookupService, LookupServiceClient>();
            //services.AddScoped<ICompanyAcDomainsService, CompanyAcDomainsServiceClient>();
            //services.AddScoped<ICompanyEventsService, CompanyEventsServiceClient>();
            services.AddScoped<IFileService, FileServiceClient>();
            services.AddScoped<IQuizResultService, QuizResultServiceClient>();
            services.AddScoped<IQuizService, QuizServiceClient>();
            services.AddScoped<IAdobeConnectAccountService, AdobeConnectAccountService>();

            //container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            //container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
            //    .DynamicParameters((k, d) => d.Add("collection", collection))
            //    .LifeStyle.Singleton);

            //container.Install(new LoggerWindsorInstaller());
            //container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());



            //services.AddOptions();

            //return WindsorRegistrationHelper.CreateServiceProvider(container, services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                //.AddDebug()
                .AddFile(Configuration.GetSection("Logging"))
                .AddFile(Configuration.GetSection("Logging_Serilog_Errors"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePages();

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
            //var origins = Configuration["AppSettings:CorsOrigin"].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            //app.UseCors(builder =>
            //    builder
            //    .WithOrigins(origins)
            //    .WithMethods("POST")
            //    .WithHeaders(new[] { "Authorization, X-Requested-With, Content-Type, Accept, Origin" })
            //    .SetPreflightMaxAge(TimeSpan.FromDays(1)));

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Events}/{action=Signup}/{id?}");
            });

           
        }
    }
}