using EdugameCloud.Lti.Api.Host.Swagger;
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
        public void ConfigureServices(IServiceCollection services)
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

    }

}
