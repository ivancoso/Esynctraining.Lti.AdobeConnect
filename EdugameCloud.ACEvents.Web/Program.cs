using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace EdugameCloud.ACEvents.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(opt =>
                    {
                        opt.AddServerHeader = false;
                    }
                )
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

        }
    }
}