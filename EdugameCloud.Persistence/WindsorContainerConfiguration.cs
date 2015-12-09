namespace EdugameCloud.Persistence
{
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;
    using Castle.Core.Resource;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    
    public static class WindsorContainerConfiguration
    {
        public static void RegisterComponents(this IWindsorContainer container)
        {
            container.Install(new NHibernateWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Mail/Esynctraining.Mail.Windsor.xml"))
            );

            container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);
            
            //container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>()
            //    .DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server)).LifeStyle.Transient);
            
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").Pick().If(Component.IsInNamespace("EdugameCloud.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("Esynctraining.Core").Pick().If(Component.IsInNamespace("Esynctraining.Core.Business.Models")).If(type => type != typeof(AuthenticationModel)).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").BasedOn(typeof(BaseConverter<,>)).WithService.Base().LifestyleTransient());            
        }

        public static void RegisterComponentsConsole(this IWindsorContainer container)
        {
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null)).LifeStyle.Singleton);            
            
        }

    }

}