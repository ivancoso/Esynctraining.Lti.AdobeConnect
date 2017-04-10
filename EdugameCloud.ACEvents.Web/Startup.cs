using System;
using System.Collections.Specialized;
using Castle.Core.Logging;
using CompanyAcDomainsNamespace;
using CompanyEventsServiceNamespace;
using EdugameCloud.Certificates.Pdf;
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
using Microsoft.Extensions.Options;
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

            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug()
                .AddFile(Configuration.GetSection("Logging"))
                .AddFile(Configuration.GetSection("Logging_Serilog_Errors"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration); // ??

            services
                .AddMvc(setup =>
                {
                    //setup.Filters.Add(new CheckModelForNullAttribute(HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })

                .AddControllersAsServices();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddSingleton<ILogger, FakeLogger>();
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);
            

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

            services.AddScoped<IQuizService>(provider =>
            {
                var quizServiceUrl = Configuration["WebServiceReferences:QuizService"];
                var client = new QuizServiceClient(QuizServiceClient.EndpointConfiguration.BasicHttpBinding_IQuizService, quizServiceUrl);
                return client;
            });

            services.AddScoped<IQuizResultService>(provider =>
            {
                var quizResultServiceUrl = Configuration["WebServiceReferences:QuizResultService"];
                var client = new QuizResultServiceClient(QuizResultServiceClient.EndpointConfiguration.BasicHttpBinding_IQuizResultService, quizResultServiceUrl);
                return client;
            });

            services.AddScoped<IFileService>(provider =>
            {
                var configOptions = provider.GetService<IOptionsSnapshot<AppSettings>>().Value;
                var fileServiceUrl = configOptions.WebServiceReferences.FileService;
                var client = new FileServiceClient(FileServiceClient.EndpointConfiguration.BasicHttpBinding_IFileService, fileServiceUrl);
                return client;
            });

            services.AddScoped<IAdobeConnectAccountService, AdobeConnectAccountService>();

            services.AddScoped<IQuizCerfificateProcessor>(provider =>
                {
                    var configOptions = provider.GetService<IOptionsSnapshot<AppSettings>>().Value;
                    var certificateSettings = configOptions.CertificateSettings;
                    var client = new QuizCerfificateProcessor(certificateSettings);
                    return client;
                });
            services.AddScoped<IGoddardApiConsumer, GoddardApiConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation($"Env var value ASPNETCORE_ENVIRONMENT={env.EnvironmentName}");

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