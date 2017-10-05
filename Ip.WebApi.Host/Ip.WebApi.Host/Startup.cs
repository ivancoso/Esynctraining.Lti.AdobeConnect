using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    //setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                //.AddApplicationPart(typeof(BaseController).Assembly)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations();

            services
                .AddSingleton(LoggerFactory)
                .AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();

            //var basicLogger = LoggerFactory.CreateLogger("Basic");
            //services
            //    .AddSingleton(basicLogger);

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

                app.UseSwaggerUI(c =>
                {
                    //(env.IsDevelopment() ? string.Empty : "/api")
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

        }

        private static IServiceCollection AddSwagger(IServiceCollection services, IHostingEnvironment env)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
            });

            return services;
        }

    }

}
