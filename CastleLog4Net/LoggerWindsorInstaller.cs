using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Esynctraining.CastleLog4Net
{
    public sealed class LoggerWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<LoggingFacility>(x =>
                x
                //.WithConfig(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
                //.ToLog("MyLogger")
                .LogUsing<Log4NetFactory>());
        }

    }

}
