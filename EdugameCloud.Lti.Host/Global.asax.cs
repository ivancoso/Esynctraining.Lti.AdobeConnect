using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.Bridge;
using EdugameCloud.Lti.Haiku;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Lti.Sakai;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.HttpClient;
using Esynctraining.Lti.Lms.AgilixBuzz;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.API.Haiku;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.API.Sakai;
using Esynctraining.Lti.Lms.Common.API.Schoology;
using Esynctraining.Lti.Lms.Desire2Learn;
using Esynctraining.Lti.Lms.Moodle;
using Esynctraining.Lti.Lms.Sakai;
using Esynctraining.Lti.Lms.Schoology;
using Esynctraining.Mvc;
using Esynctraining.WebApi;
using Esynctraining.Windsor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace EdugameCloud.Lti.Host
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var webformVE = ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault();
            ViewEngines.Engines.Remove(webformVE);

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);

            container.RegisterComponents();
            RegisterComponentsWeb(container);
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);
            //RegisterLocalComponents(container);

            SetControllerFactory(container);
            AreaRegistration.RegisterAllAreas();

            //var modelBinders = container.ResolveAll(typeof(BaseModelBinder));
            //foreach (var binder in modelBinders)
            //{
            //    var modelBinder = (BaseModelBinder)binder;
            //    if (modelBinder.BinderTypes != null)
            //    {
            //        foreach (var binderType in modelBinder.BinderTypes)
            //        {
            //            ModelBinders.Binders.Add(binderType, modelBinder);
            //        }
            //    }
            //}

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WindsorValidatorFactory(new WindsorServiceLocator(container), IoC.Resolve<ILogger>())));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DefaultModelBinder.ResourceClassKey = "Errors";
            MvcHandler.DisableMvcResponseHeader = true;

            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>(), container.Resolve<IHttpClientFactory>());


            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                new ServiceLocatorControllerActivator(new WindsorServiceLocator(container)));

            // TRICK: remove all files on start
            CachePolicies.InvalidateCache();
            
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["DefaultConnectionLimit"]);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            //hack - to use parameterized Resolve in LmsFactory. Must be removed when corresponding methods are added to IoC and IServiceLocator
            if (!container.Kernel.HasComponent(typeof(IWindsorContainer)))
                container.Register((IRegistration)Component.For<IWindsorContainer>().Instance(container).LifestyleSingleton());
        }

        // source : http://stackoverflow.com/questions/1178831/remove-server-response-header-iis7
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" HTTP Header from response
            if (null != Response)
            {
                Response.Headers.Remove("Server");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Host").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Host.Areas.Reports.Controllers"))
                .WithService.Self()
                .LifestyleTransient());
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection = RegisterHttpClients(serviceCollection);
            container.AddServices(serviceCollection);

            //moodle
            container.Register(Component.For<IMoodleApi>().ImplementedBy<MoodleApi>().LifeStyle.Transient);
            container.Register(Component.For<IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<MoodleLmsUserService>().Named(LmsProviderEnum.Moodle.ToString()).LifeStyle.Transient);

            //brightspace
            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserService>().Named(LmsProviderEnum.Brightspace.ToString()).LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserServiceSync>().Named(LmsProviderEnum.Brightspace.ToString() + "_Sync").LifeStyle.Transient);

            //canvas
            container.Register(Component.For<ICanvasAPI>().ImplementedBy<CanvasAPI>().LifeStyle.Transient);
            container.Register(Component.For<IEGCEnabledCanvasAPI>().ImplementedBy<EGCEnabledCanvasAPI>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<CanvasLmsUserService>().Named(LmsProviderEnum.Canvas.ToString()).LifeStyle.Transient);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<CanvasLmsCourseSectionsService>().Named(LmsProviderEnum.Canvas.ToString() + "SectionsService").LifeStyle.Transient);
            container.Register(Component.For<LmsCalendarEventServiceBase>().ImplementedBy<CanvasCalendarEventService>().Named(LmsProviderEnum.Canvas.ToString() + "CalendarService").LifeStyle.Transient);

            //agilixbuzz
            container.Register(Component.For<IAgilixBuzzApi>().ImplementedBy<DlapAPI>().Named("IAgilixBuzzApi").LifeStyle.Transient);
            //container.Register(Component.For<IAgilixBuzzScheduling>().ImplementedBy<ShedulingHelper>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<AgilixBuzzLmsUserService>().Named(LmsProviderEnum.AgilixBuzz.ToString()).LifeStyle.Transient);
            //container.Register(Component.For<DlapAPI>().ImplementedBy<DlapAPI>().LifeStyle.Singleton);

            //blackboard
            container.Register(Component.For<IBlackBoardApi>().ImplementedBy<SoapBlackBoardApi>().LifeStyle.Singleton);
            container.Register(Component.For<IEGCEnabledBlackBoardApi>().ImplementedBy<EGCEnabledBlackboardApi>().LifeStyle.Singleton);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BlackboardLmsUserService>().Named(LmsProviderEnum.Blackboard.ToString()).LifeStyle.Transient);

            //sakai
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SakaiLmsUserService>().Named(LmsProviderEnum.Sakai.ToString()).LifeStyle.Transient);
            container.Register(Component.For<ICalendarExportService>().ImplementedBy<SakaiCalendarExportService>().Named(LmsProviderEnum.Sakai.ToString() + "CalendarExportService").LifeStyle.Transient);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<SakaiLmsCourseSectionsService>().Named(LmsProviderEnum.Sakai.ToString() + "SectionsService").LifeStyle.Transient);

            //bridge
            container.Register(Component.For<IBridgeApi>().ImplementedBy<BridgeApi>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BridgeLmsUserService>().Named(LmsProviderEnum.Bridge.ToString()).LifeStyle.Transient);
            container.Register(Component.For<ICalendarExportService>().ImplementedBy<BridgeCalendarExportService>().Named(LmsProviderEnum.Bridge.ToString() + "CalendarExportService").LifeStyle.Transient);
            container.Register(Component.For<IMeetingSessionService>().ImplementedBy<BridgeMeetingSessionService>().Named(LmsProviderEnum.Bridge.ToString() + "SessionsService").LifeStyle.Transient);

            //schoology
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SchoologyLmsUserService>().Named(LmsProviderEnum.Schoology.ToString()).LifeStyle.Transient);
            
            //haiku
            container.Register(Component.For<HaikuOAuthBaseClient>().LifeStyle.Transient);
            container.Register(Component.For<IHaikuRestApiClient>().ImplementedBy<HaikuRestApiClient>().LifeStyle.Transient);
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<HaikuLmsUserService>().Named(LmsProviderEnum.Haiku.ToString()).LifeStyle.Transient);
            container.Register(Component.For<LmsCourseSectionsServiceBase>().ImplementedBy<HaikuLmsCourseSectionsService>().Named(LmsProviderEnum.Haiku.ToString() + "SectionsService").LifeStyle.Transient);

            //container.Install(
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.AgilixBuzz/EdugameCloud.Lti.AgilixBuzz.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.DialogEdu/EdugameCloud.Lti.DialogEdu.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Bridge/EdugameCloud.Lti.Bridge.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Schoology/EdugameCloud.Lti.Schoology.Windsor.xml")),
            //Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Haiku/EdugameCloud.Lti.Haiku.Windsor.xml"))
            //);

            container.Install(new LtiWindsorInstaller());
            container.Install(new LtiMvcWindsorInstaller());
            container.Install(new TelephonyWindsorInstaller());

            //container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

        private static ServiceCollection RegisterHttpClients(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<DebugHttpLoggingHandler>();
            serviceCollection.AddHttpClient();
            serviceCollection.AddHttpClient(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientName, c =>
            {
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.MoodleApiClientTimeout);
            });
            serviceCollection.AddHttpClient<BuzzApiClient>(c =>
            {
                c.BaseAddress = new Uri(WebConfigurationManager.AppSettings["AgilixBuzzApiUrl"]);
                c.Timeout = TimeSpan.FromMilliseconds(Esynctraining.Lti.Lms.Common.Constants.Http.BuzzApiClientTimeout);
                c.DefaultRequestHeaders.Add("User-Agent", "EduGameCloud");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    CookieContainer = new System.Net.CookieContainer()
                };
            }).AddHttpMessageHandler<DebugHttpLoggingHandler>();

            serviceCollection.AddHttpClient<LTI2Api>();
            serviceCollection.AddHttpClient<IEGCEnabledSakaiApi, SakaiApi>();

            serviceCollection.AddHttpClient<ISchoologyRestApiClient, SchoologyRestApiClient>(c =>
            {
                c.BaseAddress = new Uri(WebConfigurationManager.AppSettings["SchoologyApiUrl"]);
            });

            serviceCollection.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, CustomLoggingFilter>());

            return serviceCollection;
        }

        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

    }

}