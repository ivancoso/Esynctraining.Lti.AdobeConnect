namespace EdugameCloud.Persistence
{
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Core;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;

    using FluentValidation;

    /// <summary>
    /// The windsor container configuration.
    /// </summary>
    public static class WindsorContainerConfiguration
    {
        public static void RegisterComponents(this IWindsorContainer container, bool console = false, bool web = false)
        {
            container.Install(new CoreWindsorInstaller());
            container.Install(new NHibernateWindsorInstaller());
            container.Install(new MailWindsorInstaller());

            if (console)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);   
            }
            else if (web)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);
            }
            
            container.Register(Component.For(typeof(RealTimeNotificationModel)).ImplementedBy(typeof(RealTimeNotificationModel)).LifeStyle.Transient);
            
            if (!console)
            {
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);
            }
            else
            {
                container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", (GlobalizationSection)null)).LifeStyle.Singleton);
            }

            container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>().DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server)).LifeStyle.Transient);
            
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").Pick().If(Component.IsInNamespace("EdugameCloud.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("Esynctraining.Core").Pick().If(Component.IsInNamespace("Esynctraining.Core.Business.Models")).If(type => type != typeof(AuthenticationModel)).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Core").BasedOn(typeof(BaseConverter<,>)).WithService.Base().LifestyleTransient());

            if (web)
            {
                container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWebRequest);
                // not in use container.Register(Component.For<XDocumentWrapper>().LifeStyle.Transient);
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
                container.Register(Classes.FromAssemblyNamed("EdugameCloud.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            }
        }

    }

}