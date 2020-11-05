using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AnonymousChat.WebApi.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(".\\Logs\\{Date}.txt")
                .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
            //.CreateDefaultBuilder(args)
            .UseKestrel(opt =>
            {
                opt.AddServerHeader = false;
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            //.ConfigureLogging((hostingContext, logging) =>
            //{
            //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //})
            .UseStartup<Startup>()
            .UseSerilog()
            .UseHealthChecks("/health", TimeSpan.FromSeconds(10))
            .Build();
    }

}
