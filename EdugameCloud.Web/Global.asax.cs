namespace EdugameCloud.Web
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
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
            IoC.Initialize(new WindsorContainer());
            IoC.Container.RegisterComponents(web: true);
            RegisterLocalComponents(IoC.Container);
            SetControllerFactory(IoC.Container);
            AreaRegistration.RegisterAllAreas();
            var modelBinders = IoC.Container.ResolveAll(typeof(BaseModelBinder));
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
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WindsorValidatorFactory(IoC.Container)));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DefaultModelBinder.ResourceClassKey = "Errors";
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
            if (FormsAuthentication.CookiesSupported)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        Tuple<int, string> idEmailHash;
                        if (AuthenticationModel.ParseCookieValue(Request.Cookies[FormsAuthentication.FormsCookieName].Value, out idEmailHash))
                        {
                            var user = IoC.Resolve<UserModel>().GetOneById(idEmailHash.Item1).Value;
                            if (user.HashEmail() == idEmailHash.Item2)
                            {
                                e.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(user.Email, "Forms"), new[] { user.UserRole.UserRoleName });
                            }
                            else
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
                else
                {
                    
                }

            }
        }
    }
}