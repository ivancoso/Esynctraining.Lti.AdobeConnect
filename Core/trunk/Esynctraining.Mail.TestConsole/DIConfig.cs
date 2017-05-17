using System;
using System.Collections.Specialized;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Esynctraining.Mail.TestConsole
{
    public static class DIConfig
    {
        public static IServiceCollection RegisterComponents()
        {
            var services = new ServiceCollection()
                
                .AddOptions()

                .Configure<SmtpSettings>(Program.Configuration.GetSection("Smtp"))
                .AddSingleton<SmtpSettings>((sp) => sp.GetService<IOptions<SmtpSettings>>().Value)

                 .Configure<NotificationsSettings>(Program.Configuration.GetSection("Mailer"))
                .AddSingleton<INotificationsSettings>((sp) => sp.GetService<IOptions<NotificationsSettings>>().Value)

                //.AddTransient<Application>()
                ;

            return services;
        }

    }

}