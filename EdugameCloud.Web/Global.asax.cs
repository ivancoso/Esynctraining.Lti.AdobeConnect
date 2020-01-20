using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
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
using Esynctraining.Windsor;
using FluentValidation;
using FluentValidation.Mvc;
using Microsoft.Extensions.DependencyInjection;
using IResourceProvider = Esynctraining.Core.Providers.IResourceProvider;

namespace EdugameCloud.Web
{
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

        protected void Application_End()
        {
            WindsorIoC.Container.Dispose();
        }

        protected void Application_Start()
        {
            var webformVE = ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault();
            ViewEngines.Engines.Remove(webformVE);

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);

            container.RegisterComponents();
            RegisterLtiComponents(container);
            RegisterComponentsWeb(container);
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLocalComponents(container);
            RegisterHttpClient(container);
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
            DefaultModelBinder.ResourceClassKey = "Errors";
            MvcHandler.DisableMvcResponseHeader = true;

            string pathPropertiesPath = this.GetPathPropertiesPath();
            container.Register(Component.For<FlexSettingsProvider>().ImplementedBy<FlexSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", FlexSettingsProvider.ReadSettings(pathPropertiesPath))).LifeStyle.Singleton);
        }


        // NOTE: for meeting-host-report only
        private static void RegisterLtiComponents(WindsorContainer container)
        {
            //container.Install(
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.AgilixBuzz/EdugameCloud.Lti.AgilixBuzz.Windsor.xml")),
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml"))
            //    Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Schoology/EdugameCloud.Lti.Schoology.Windsor.xml")),
            //);

            container.Install(new LtiWindsorInstaller());
            //container.Install(new LtiMvcWindsorInstaller());
            //container.Install(new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

        public static void RegisterComponentsWeb(IWindsorContainer container)
        {
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.PerWebRequest);

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);


            container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWebRequest);
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").Pick().If(Component.IsInNamespace("EdugameCloud.MVC.Controllers")).WithService.Self().LifestyleTransient());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.MVC").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Web").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }
        
        private string GetPathPropertiesPath()
        {
            string @path = @"Content\swf\config\paths.properties";
            return Path.Combine(HttpContext.Current.Server.MapPath("~"), @path);
        }

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
        private static void RegisterHttpClient(IWindsorContainer container)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            container.AddServices(serviceCollection);
        }

        private static void SetControllerFactory(IWindsorContainer container)
        {
            var controllerFactory = new ServiceLocatorControllerFactory(new WindsorServiceLocator(container));
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

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
                                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
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
                else if (FormsAuthentication.CookiesSupported && HasKey(Request.Cookies, FormsAuthentication.FormsCookieName))
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
                            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                                Request.Cookies[FormsAuthentication.FormsCookieName].Value = null;
                        }
                    }
                    else
                    {
                        if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
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


        private static bool HasKey(HttpCookieCollection nvc, string key)
        {
            if (nvc != null && nvc.AllKeys.Length > 0)
            {
                return nvc.AllKeys.Any(keyVar => keyVar.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

    }

}