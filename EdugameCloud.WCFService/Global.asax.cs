namespace EdugameCloud.WCFService
{
    using System;
    using System.Web;
    using Castle.Core.Logging;
    using Castle.Windsor;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Utils;

    public class Global : HttpApplication
    {
        #region Methods

        /// <summary>
        /// The application error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            IoC.Resolve<ILogger>().Error("Unhandled exception: ", this.Server.GetLastError());
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_Start(object sender, EventArgs e)
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            DIConfig.RegisterComponents(container);
        }

        #endregion
    }
}