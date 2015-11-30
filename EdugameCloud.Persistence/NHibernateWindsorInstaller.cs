using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Esynctraining.NHibernate;
using NHibernate;
using NHibernate.Cfg;

namespace EdugameCloud.Persistence
{
    public sealed class NHibernateWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());

            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
        }

    }

}
