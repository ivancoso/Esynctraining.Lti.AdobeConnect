namespace EdugameCloud.WCFService
{
    using System;
    using System.Web;
    using Esynctraining.Core.Logging;
    using Castle.Windsor;

    using Esynctraining.Core.Utils;
    using Esynctraining.Windsor;
    using Core.Business;

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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // for dev only to allo preflight requests (on jquery, angular etc)
            //if (HttpContext.Current.Request.HttpMethod == "OPTIONS" && HttpContext.Current.Request.UrlReferrer != null && (HttpContext.Current.Request.UrlReferrer.DnsSafeHost.StartsWith("dev") || HttpContext.Current.Request.UrlReferrer.DnsSafeHost.StartsWith("localhost") || HttpContext.Current.Request.UrlReferrer.DnsSafeHost.StartsWith("goddard")))
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE");

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                HttpContext.Current.Response.End();
            }
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
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            // TRICK: remove all files on start
            CachePolicies.InvalidateCache();
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