using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Core.Logging
{
    public sealed class LoggerWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>().ImplementedBy<Logger>().LifeStyle.Singleton);
            container.Register(Component.For<ILogger>().Named("Monitoring").ImplementedBy<MonitoringLogger>().LifeStyle.Singleton);
        }

    }

}
