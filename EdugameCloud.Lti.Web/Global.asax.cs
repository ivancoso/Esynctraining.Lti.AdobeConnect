using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.BrainHoney;
using EdugameCloud.Lti.Canvas;
using EdugameCloud.Lti.Desire2Learn;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Lti.Web.Providers;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Web
{
    /// <summary>
    /// The MVC application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The application_ end.
        /// </summary>
        protected void Application_End()
        {
            IoC.Container.Dispose();
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            DiConfig.RegisterComponents(container);
            RegisterLtiComponents(container);
            SetControllerFactory(container);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>());
        }

        /// <summary>
        /// The register LTI components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));

            // TODO: every LMS
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.BrainHoney").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Canvas").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Moodle").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.BlackBoard").BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());

            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyScheduling>().ImplementedBy<ShedulingHelper>());
            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyApi>().ImplementedBy<DlapAPI>().Named("IBrainHoneyApi"));

            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.ICanvasAPI>().ImplementedBy<CanvasAPI>().Named("ICanvasAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.IEGCEnabledCanvasAPI>().ImplementedBy<EGCEnabledCanvasAPI>().Named("IEGCEnabledCanvasAPI"));

            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IMoodleApi>().ImplementedBy<MoodleApi>().Named("IMoodleAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().Named("IEGCEnabledMoodleAPI"));

            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IBlackBoardApi>().ImplementedBy<SoapBlackBoardApi>().Named("IBlackBoardAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IEGCEnabledBlackBoardApi>().ImplementedBy<EGCEnabledBlackboardApi>().Named("IEGCEnabledBlackBoardAPI"));

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").Pick().If(Component.IsInNamespace("EdugameCloud.Lti.Controllers")).WithService.Self().LifestyleTransient());
        }

        /// <summary>
        /// The set controller factory.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new WindsorControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}