using System.Collections.Specialized;
using System.Reflection;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Core.Providers;
using Esynctraining.Json.Jil;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Zoom.Api;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Lti.Zoom.Routes;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Esynctraining.Lti.Zoom.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = Assembly.Load(new AssemblyName("Esynctraining.Lti.Zoom"));
            services
                .AddMvc()
                .AddApplicationPart(controllerAssembly)
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddSingleton<Esynctraining.Core.Logging.ILogger, MicrosoftLoggerWrapper>();

            var configurationSection = Configuration.GetSection("AppSettings");
            var settings = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            services
                .AddSingleton(new ApplicationSettingsProvider(settings));

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            services.AddDbContext<ZoomDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ZoomDb")));
            services.AddTransient<ILmsLicenseService, LmsLicenseDbService>();
            services.AddTransient<UserSessionService, UserSessionService>();
            services.AddSingleton<IJsonSerializer, JilSerializer>();
            services.AddSingleton<IJsonDeserializer, JilSerializer>();
            services.AddTransient<IEGCEnabledCanvasAPI, EGCEnabledCanvasAPI>();
            services.AddTransient<LmsUserServiceBase, CanvasLmsUserService>();
            services.AddTransient<ZoomUserService, ZoomUserService>();
            services.AddTransient<ZoomReportService, ZoomReportService>();
            services.AddTransient<ZoomMeetingService, ZoomMeetingService>();
            services.AddTransient<ZoomOfficeHoursService, ZoomOfficeHoursService>();

            services.AddSingleton<ILtiTokenAccessor, QueryStringTokenAccessor>();
            services.AddScoped<ZoomLicenseAccessor, ZoomLicenseAccessor>();
            services.AddScoped<IZoomOptionsAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ILmsLicenseAccessor, ZoomLicenseAccessor>(s => s.GetService<ZoomLicenseAccessor>());
            services.AddScoped<ZoomApiWrapper, ZoomApiWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(LtiRoutes.AppendTo);


        }
    }
}
