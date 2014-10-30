namespace EdugameCloud.Web
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Extensions;
//    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.MVC.ModelBinders;
    using EdugameCloud.MVC.Providers;
    using EdugameCloud.Persistence;
    using EdugameCloud.Web.App_Start;
    using EdugameCloud.Web.Providers;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using FluentValidation.Mvc;

    using IResourceProvider = Esynctraining.Core.Providers.IResourceProvider;

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
            container.Register(Component.For<DlapAPI>().ImplementedBy<DlapAPI>());
            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
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

            if (Request.QueryString != null && Request.QueryString.HasKey("egcSession"))
            {
                try
                {
                    var userModel = IoC.Resolve<UserModel>();
                    var user = userModel.GetOneByToken(Request.QueryString["egcSession"]).Value;
                    if (user != null && user.SessionTokenExpirationDate.HasValue
                        && user.SessionTokenExpirationDate > DateTime.Now)
                    {
                        e.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(user.Email, "Forms"), new[] { user.UserRole.UserRoleName });
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
                }
            }
            else if (FormsAuthentication.CookiesSupported && Request.Cookies != null
                     && Request.Cookies.HasKey(FormsAuthentication.FormsCookieName))
            {
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = null;
                try
                {
                    ticket = FormsAuthentication.Decrypt(cookie.Value);

                }
                catch (Exception decryptError)
                {
                }

                if (ticket == null) return; // Not authorised
                if (ticket.Expiration > DateTime.Now)
                {
                    var userModel = IoC.Resolve<UserModel>();
                    var user = userModel.GetOneByEmail(ticket.Name).Value;
                    if (user != null)
                    {
                        e.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(user.Email, "Forms"), new[] { user.UserRole.UserRoleName });
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