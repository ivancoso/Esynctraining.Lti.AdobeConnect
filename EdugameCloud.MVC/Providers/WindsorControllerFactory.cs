namespace EdugameCloud.MVC.Providers
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.Windsor;

    /// <summary>
    /// The windsor controller factory.
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        #region Fields

        /// <summary>
        /// The container.
        /// </summary>
        private readonly IWindsorContainer container;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public WindsorControllerFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get controller instance.
        /// </summary>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <param name="controllerType">
        /// The controller type.
        /// </param>
        /// <returns>
        /// The <see cref="IController"/>.
        /// </returns>
        /// <exception cref="HttpException">
        ///  404 controller not found
        /// </exception>
        /// <exception cref="ArgumentException">
        ///  Controller instance does not implement IController
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Controller is not registered with IoC container
        /// </exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;
            if (controllerType == null)
            {
                string notController = string.Format(
                    CultureInfo.CurrentUICulture, 
                    "The controller for path '{0}'was not found or does not implement IController.", 
                    new object[] { requestContext.HttpContext.Request.Path });
                throw new HttpException(0x194, notController);
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentUICulture, 
                        "The controller type '{0}' must implement IController.", 
                        new object[] { controllerType }), 
                    "controllerType");
            }

            try
            {
                controller = (IController)this.container.Resolve(controllerType);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentUICulture, 
                        "An error occurred when trying to create a controller of type '{0}'. Make sure that IoC container contains all necessary registrations.", 
                        new object[] { controllerType }), 
                    exception);
            }

            return controller;
        }

        #endregion
    }
}