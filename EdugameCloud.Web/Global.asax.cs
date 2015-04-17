using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.BrainHoney;
using EdugameCloud.Lti.Canvas;

namespace EdugameCloud.Web
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Esynctraining.Core.Extensions;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.MVC.ModelBinders;
    using EdugameCloud.MVC.Providers;
    using EdugameCloud.Persistence;
    using EdugameCloud.Web.Providers;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using FluentValidation.Mvc;

    using IResourceProvider = Esynctraining.Core.Providers.IResourceProvider;
    using Castle.Core.Logging;
    using System.Security.Principal;
    using EdugameCloud.Lti.Moodle;
    using EdugameCloud.Lti.BlackBoard;

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
            container.RegisterComponents(web: true);
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
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WindsorValidatorFactory(container)));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DefaultModelBinder.ResourceClassKey = "Errors";

            string pathPropertiesPath = this.GetPathPropertiesPath();
            container.Register(Component.For<FlexSettingsProvider>().ImplementedBy<FlexSettingsProvider>().DynamicParameters((k, d) => d.Add("collection", FlexSettingsProvider.ReadSettings(pathPropertiesPath))).LifeStyle.Singleton);
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
            var controllerFactory = new WindsorControllerFactory(container);
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
    }
}