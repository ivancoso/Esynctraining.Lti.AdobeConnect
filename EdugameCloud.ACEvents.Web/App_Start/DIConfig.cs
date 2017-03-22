using System.Configuration;
using System.Web.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Providers;


namespace EdugameCloud.ACEvents.Web
{
    public class DIConfig
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            container.Install(new Esynctraining.CastleLog4Net.LoggerWindsorInstaller());
            container.Install(new Esynctraining.Core.Logging.CastleLogger.LoggerWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.AdobeConnect/Esynctraining.AdobeConnect.Windsor.xml"))
            );

            container.Register(Component.For<ICache>().ImplementedBy<MemoryCacheWrapper>().LifeStyle.Singleton);

            //container.Register(Component.For<ApplicationSettingsProvider>()
            //.ImplementedBy<ApplicationSettingsProvider>()
            //.DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
            //.LifestyleSingleton());

            //container.Install(new NHibernateWindsorInstaller());
            //container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWebRequest);

            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            //container.RegisterEgcComponents();
        }

    }
}