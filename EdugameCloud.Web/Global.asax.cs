using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti;
using EdugameCloud.MVC.ModelBinders;
using EdugameCloud.Persistence;
using EdugameCloud.Web.Providers;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Business.Models;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.FluentValidation;
using Esynctraining.Mvc;
using Esynctraining.Windsor;
using FluentValidation;
using FluentValidation.Mvc;
using IResourceProvider = Esynctraining.Core.Providers.IResourceProvider;

namespace EdugameCloud.Web
{
    /// <summary>
    /// The MVC application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        //public MvcApplication()
        //{
        //    #region Request life cycle

        //    this.BeginRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Begin Request");
        //    this.AuthenticateRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Authenticate Request");
        //    this.PostAuthenticateRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Authenticate Request");
        //    this.AuthorizeRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Authorize Request");
        //    this.PostAuthorizeRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Authorize Request");
        //    this.ResolveRequestCache += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Resolve Request Cache");
        //    this.PostResolveRequestCache += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Resolve Request Cache");
        //    this.MapRequestHandler += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Map Request Handler");
        //    this.PostMapRequestHandler += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Map Request Handler");
        //    this.AcquireRequestState += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Acquire Request State");
        //    this.PostAcquireRequestState += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Acquire Request State");
        //    this.PreRequestHandlerExecute += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Pre Request Handler Execute");
        //    // page events
        //    this.PostRequestHandlerExecute += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Request Handler Execute");
        //    this.ReleaseRequestState += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Release Request State");
        //    this.PostReleaseRequestState += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Release Request State");
        //    this.UpdateRequestCache += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Update Request Cache");
        //    this.PostUpdateRequestCache += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Update Request Cache");
        //    this.LogRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Log Request");
        //    this.PostLogRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - Post Log Request");
        //    this.EndRequest += (x, y) => IoC.Resolve<ILogger>().Info("Global.asax - End Request");
            
        //    #endregion
        //}

        /// <summary>
        /// The application_ end.
        /// </summary>
        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
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
            RegisterLocalComponents(container);

            SetControllerFactory(container);
            AreaRegistration.RegisterAllAreas();
            var modelBinders = container.ResolveAll(typeof(BaseModelBinder));
            foreach (var binder in modelBinders)
            {
                var modelBinder = (BaseModelBinder)binder;
                if (modelBinder.BinderTypes != null)
                {
                    foreach (var binderType in modelBinder.BinderTypes)
                    {
                        ModelBinders.Binders.Add(binderType, modelBinder);
                    }
                }
            }

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WindsorValidatorFactory(new WindsorServiceLocator(container), IoC.Resolve<ILogger>())));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DefaultModelBinder.ResourceClassKey = "Errors";
            MvcHandler.DisableMvcResponseHeader = true;

            string pathPropertiesPath = this.GetPathPropertiesPath();
            container.Register(Component.For<FlexSettingsProvider>().ImplementedBy<FlexSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", FlexSettingsProvider.ReadSettings(pathPropertiesPath))).LifeStyle.Singleton);
            AuthConfig.RegisterAuth(container.Resolve<ApplicationSettingsProvider>());


            //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().Single());
            //ValueProviderFactories.Factories.Add(new MyJsonValueProviderFactory());


            // TRICK: remove all files on start
            CachePolicies.InvalidateCache();
        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            // NOTE: is in container.RegisterComponents();
            //container.Install(
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://Esynctraining.Core/Esynctraining.Core.Windsor.xml"))
            //);

            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);


            container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWebRequest);
            // not in use container.Register(Component.For<XDocumentWrapper>().LifeStyle.Transient);
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

        /// <summary>
        /// The register LTI components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.BrainHoney/EdugameCloud.Lti.BrainHoney.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());
            container.Install(new LtiMvcWindsorInstaller());
            container.Install(new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());

            //container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());
        }

        /// <summary>
        /// The get path properties path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetPathPropertiesPath()
        {
            string @path = @"Content\swf\config\paths.properties";
            return Path.Combine(HttpContext.Current.Server.MapPath("~"), @path);
        }

        /// <summary>
        /// The register local components.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void RegisterLocalComponents(IWindsorContainer container)
        {
            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.MVC")
                    .BasedOn(typeof(BaseModelBinder))
                    .WithService.Base()
                    .LifestyleTransient());
            container.Register(
                Component.For<IResourceProvider>()
                    .ImplementedBy<EGCResourceProvider>()
                    .Activator<ResourceProviderActivator>());
        }

        /// <summary>
        /// The set controller factory.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        /// <summary>
        /// The forms authentication_ on authenticate.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
        {
            //var sw = Stopwatch.StartNew();
            try
            {
                if (Request.QueryString.HasKey("egcSession"))
                {
                    try
                    {
                        var userModel = IoC.Resolve<UserModel>();
                        var user = userModel.GetOneByToken(Request.QueryString["egcSession"]).Value;
                        if (user != null && user.SessionTokenExpirationDate.HasValue
                            && user.SessionTokenExpirationDate > DateTime.Now)
                        {
                            e.User = new GenericPrincipal(new GenericIdentity(user.Email, "Forms"), new[] { user.UserRole.UserRoleName });
                            if (FormsAuthentication.CookiesSupported)
                            {
                                FormsAuthentication.SetAuthCookie(user.Email, false);
                                Request.Cookies[FormsAuthentication.FormsCookieName].Expires = user.SessionTokenExpirationDate.Value;
                            }
                        }
                        else
                        {
                            if (FormsAuthentication.CookiesSupported)
                            {
                                Request.Cookies[FormsAuthentication.FormsCookieName].Value = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //somehting went wrong
                        IoC.Resolve<ILogger>().Error("FormsAuthentication_OnAuthenticate", ex);
                    }
                }
                else if (FormsAuthentication.CookiesSupported && Request.Cookies.HasKey(FormsAuthentication.FormsCookieName))
                {
                    var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket ticket = null;
                    try
                    {
                        ticket = FormsAuthentication.Decrypt(cookie.Value);
                    }
                    catch (Exception ex)
                    {
                        IoC.Resolve<ILogger>().Error("FormsAuthentication_OnAuthenticate:FormsAuthentication.Decrypt", ex);
                    }

                    if (ticket == null) return; // Not authorised
                    if (ticket.Expiration > DateTime.Now)
                    {
                        var userModel = IoC.Resolve<UserModel>();
                        var user = userModel.GetByEmailWithRole(ticket.Name);
                        if (user != null)
                        {
                            e.User = new GenericPrincipal(new GenericIdentity(user.Email, "Forms"), new[] { user.UserRole.UserRoleName });
                        }
                        else
                        {
                            Request.Cookies[FormsAuthentication.FormsCookieName].Value = null;
                        }
                    }
                    else
                    {
                        Request.Cookies[FormsAuthentication.FormsCookieName].Value = null;
                    }

                }

            }
            finally
            {
                //sw.Stop();
                //var time = sw.Elapsed;
                //IoC.Resolve<ILogger>().InfoFormat("FormsAuthentication_OnAuthenticate: time: {0}.", time.ToString());
            }
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

    }

}