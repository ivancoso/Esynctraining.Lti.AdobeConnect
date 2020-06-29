using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;
using Esynctraining.AspNetCore.Formatters;

namespace Esynctraining.Lti.Reports.Host.Core
{
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
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationSection = Configuration.GetSection("AppSettings");
            var appSettings = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                appSettings.Add(appSetting.Key, appSetting.Value);
            }

            services
                .AddSingleton(new ApplicationSettingsProvider(appSettings));


            services.AddMvcCore(setup =>
            {
                setup.InputFormatters.Insert(0, new JilInputFormatter());
                setup.OutputFormatters.Insert(0, new JilOutputFormatter());
            });
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
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
