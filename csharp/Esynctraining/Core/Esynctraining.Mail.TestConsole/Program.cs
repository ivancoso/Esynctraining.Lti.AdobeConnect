using System;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Esynctraining.Mail.TestConsole
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }


        public static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .AddJsonFile(@"appSettings.json", false);
            Configuration = builder.Build();

            IServiceCollection services = DIConfig.RegisterComponents();
            var sp = services.BuildServiceProvider();

            var smtp = sp.GetService<SmtpSettings>();
            var notifications = sp.GetService<INotificationsSettings>();

            var defaultMail = notifications.RecipientSettings.GetByToken("default-email");
            var from = notifications.SystemEmails.GetByToken(defaultMail.FromToken);

            var app = sp.GetService<Application>();
            app.Run();

        }
        
    }
}