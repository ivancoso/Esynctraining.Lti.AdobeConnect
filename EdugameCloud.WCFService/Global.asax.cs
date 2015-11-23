namespace EdugameCloud.WCFService
{
    using System;
    using System.Web;
    using Esynctraining.Core.Logging;
    using Castle.Windsor;

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

        // source : http://stackoverflow.com/questions/1178831/remove-server-response-header-iis7
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" HTTP Header from response
            if (null != Response)
            {
                Response.Headers.Remove("Server");
            }
        }

        #endregion

    }

}