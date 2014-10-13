
namespace EdugameCloud.Canvas
{
    using System.Web.Optimization;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Persistence;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Utils;

    using NHibernate;

    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using NHibernate.Cfg;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);

            container.Register(Component.For<Persistence.FluentConfiguration>().LifeStyle.Singleton);

            container.Register(Component.For<CanvasCourseMeetingModel>().ImplementedBy<CanvasCourseMeetingModel>().LifeStyle.Transient);
            container.Register(
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            container.Register(
                Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(
                Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(
                Component.For<Persistence.ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);

            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.Core")
                    .Pick()
                    .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));
            //container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}