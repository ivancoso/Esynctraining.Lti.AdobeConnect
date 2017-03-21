using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EdugameCloud.Lti.Api.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var container = new WindsorContainer();
            //WindsorIoC.Initialize(container);

            //container.RegisterComponents();
            //RegisterComponentsWeb(container);
            //container.Install(new LoggerWindsorInstaller());
            //container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            //RegisterLtiComponents(container);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
