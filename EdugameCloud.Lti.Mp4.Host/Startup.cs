using System;
using System.Net;
using System.Threading.Tasks;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor.MsDependencyInjection;
using Esynctraining.AspNetCore;
using Esynctraining.AspNetCore.Filters;
using Esynctraining.AspNetCore.Formatters;
using Esynctraining.Json.Jil;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdugameCloud.Lti.Mp4.Host
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "mp4subtitlebyfilescoidcontentsavepost")
            {
                while (operation.Parameters.Count > 1)
                    operation.Parameters.RemoveAt(1);
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file",
                    In = "formData",
                    Description = "Upload File",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }


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
                    setup.Filters.Add(new ValidateModelAttribute(LoggerFactory, new JilSerializer(), HostingEnvironment.IsDevelopment()));
                    setup.Filters.Add(new GlobalExceptionFilterAttribute(LoggerFactory, HostingEnvironment.IsDevelopment()));
                })
                .AddApplicationPart(typeof(Controllers.Mp4Controller).Assembly)
                .AddControllersAsServices()
                //.AddJsonFormatters()
                .AddApiExplorer()
                .AddDataAnnotations();

            services.AddMemoryCache();

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            var container = DIConfig.ConfigureWindsor(Configuration);
            services.AddRequestScopingMiddleware(container.BeginScope);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "eSyncTraining LTI MP4 API", Version = "v1" });
                //c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                //c.MapType<DateTime?>(() =>
                //    new Schema
                //    {
                //        Type = "integer",
                //        Format = "int64",
                //        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                //        Description = "MillisecondsSinceUnixEpoch",
                //        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                //        //Pattern = "pattern",
                //    }
                //);

                //c.MapType<DateTime>(() =>
                //    new Schema
                //    {
                //        Type = "integer",
                //        Format = "int64",
                //        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                //        Description = "MillisecondsSinceUnixEpoch",
                //        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                //        //Pattern = "pattern",
                //    }
                //);

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

                c.OperationFilter<FileUploadOperation>(); //Register File Upload Operation Filter

            });

            services.AddOptions();

            return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            ServicePointManager.DefaultConnectionLimit = int.Parse(Configuration["AppSettings:ConnectionBatchSize"]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next =>
            {
                return ctx =>
                {
                    // TODO: env.IsDevelopment() ? new[] { "*" } : new[] { "*" }
                    //ctx.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    //ctx.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Authorization, X-Requested-With, Content-Type, Accept, Origin" });
                    //ctx.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST" });
                    //ctx.Response.Headers.Add("Access-Control-Max-Age", new[] { "86400" }); // 1day

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "eSyncTraining LTI MP4 API V1");
            });
        }

    }

}
