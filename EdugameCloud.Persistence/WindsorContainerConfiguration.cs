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
        public static void RegisterComponents(this IWindsorContainer container)
        {
            container.Install(new NHibernateWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            );

            //container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);

            container.RegisterEgcComponents();
        }

        public static void RegisterEgcComponents(this IWindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                .WithService.Self()
                .Configure(c => c.LifestyleTransient()));

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core")
                .BasedOn(typeof(BaseConverter<,>)).WithService.Base().LifestyleTransient());
        }

        public static void RegisterComponentsConsole(this IWindsorContainer container)
        {
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                //.DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null))
                .LifeStyle.Singleton);
            
        }

    }

}