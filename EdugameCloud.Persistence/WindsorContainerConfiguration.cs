namespace EdugameCloud.Persistence
{
    using System.Configuration;
    using Castle.Core.Resource;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using EdugameCloud.Core.Converters;
    using Esynctraining.Core.Providers;

    public static class WindsorContainerConfiguration
    {
        public static IWindsorContainer RegisterComponents(this IWindsorContainer container)
        {
            container
                .Install(
                new NHibernateWindsorInstaller(),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            )
            .RegisterEgcComponents();

            return container;
        }

        public static IWindsorContainer RegisterEgcComponents(this IWindsorContainer container)
        {
            container
                .Register(
                Classes.FromAssemblyNamed("EdugameCloud.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                .WithService.Self()
                .Configure(c => c.LifestyleTransient()),

                Classes.FromAssemblyNamed("EdugameCloud.Core")
                .BasedOn(typeof(BaseConverter<,>))
                .WithService.Base().LifestyleTransient()
                );

            return container;
        }

        public static IWindsorContainer RegisterComponentsConsole(this IWindsorContainer container)
        {
            container
                .Register(
                Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient,
            
                Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                .LifeStyle.Singleton
                );

            return container;
        }

    }

}