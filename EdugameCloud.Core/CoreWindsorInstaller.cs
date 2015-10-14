using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Core
{
    public sealed class CoreWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ICache>().ImplementedBy<MemoryCacheWrapper>().LifeStyle.Singleton);

            //container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);
        }

    }

}
