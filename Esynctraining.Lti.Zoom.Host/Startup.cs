﻿using System.Collections.Specialized;
using Esynctraining.Core.Logging.MicrosoftExtensionsLogger;
using Esynctraining.Lti.Zoom.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Esynctraining.Core.Providers;

namespace Esynctraining.Lti.Zoom.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

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
            app.UseCookiePolicy();

            app.UseMvc(LtiRoutes.AppendTo);
        }
    }
}
