using System;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;

namespace EdugameCloud.PublicApi
{
    public class DIConfig
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
                //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml"))
            );

            container.Install(new NHibernateWindsorInstaller());
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);
            
            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.RegisterEgcComponents();
        }

    }

}
