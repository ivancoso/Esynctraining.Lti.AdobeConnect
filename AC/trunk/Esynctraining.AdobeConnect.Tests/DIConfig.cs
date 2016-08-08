using System.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;

namespace Esynctraining.AdobeConnect.Tests
{
    public class DIConfig
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            container.Install(new LoggerWindsorInstaller());
            container.Install(new Esynctraining.Core.Logging.CastleLogger.LoggerWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml"))
            );

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.AdobeConnect/Esynctraining.AdobeConnect.Windsor.xml"))
            );

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                    .LifeStyle.Singleton);
        }

    }


}
