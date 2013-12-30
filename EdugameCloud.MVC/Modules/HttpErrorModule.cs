namespace EdugameCloud.MVC.Modules
{
    using System;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.Core.Logging;
    using Esynctraining.Core.Utils;

    using ErrorController = EdugameCloud.MVC.Controllers.ErrorController;

    /// <summary>
    /// The http error module.
    /// </summary>
    public class HttpErrorModule : IHttpModule
    {
        #region Static Fields

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object LockObject = new object();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// The initialization.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Init(HttpApplication context)
        {
            context.Error += OnApplicationError;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get exception message.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetExceptionMessage(Exception ex)
        {
            string userName = "user is anonymous";

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userName = HttpContext.Current.User.Identity.Name;
            }

            return string.Format(
                "Message: {0}\nRequest URL: {1}.\nIP: {2}.\nUser login: {3}.", 
                ex.Message, 
                HttpContext.Current.Request.Url, 
                HttpContext.Current.Request.UserHostAddress, 
                userName);
        }

        /// <summary>
        /// The is custom error mode.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsCustomErrorMode()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (CustomErrorsSection)config.GetSection("system.web/customErrors");

            return (section.Mode == CustomErrorsMode.RemoteOnly && !HttpContext.Current.Request.IsLocal)
                   || section.Mode == CustomErrorsMode.On;
        }

        /// <summary>
        /// The on application error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnApplicationError(object sender, EventArgs e)
        {
            try
            {
                var application = (HttpApplication)sender;
                Exception exception = application.Server.GetLastError();

                if (exception != null)
                {
                    lock (LockObject)
                    {
                        var logger = IoC.Resolve<ILogger>();

                        if (exception.InnerException != null)
                        {
                            logger.Error(GetExceptionMessage(exception.InnerException), exception.InnerException);
                        }
                        else
                        {
                            logger.Error(GetExceptionMessage(exception), exception);
                        }
                    }
                }

                if (IsCustomErrorMode())
                {
                    HttpContext.Current.Response.Clear();
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", EdugameCloudT4.Error.Name);
                    routeData.Values.Add("action", EdugameCloudT4.Error.ActionNames.Index);
                    routeData.Values.Add("exc", exception);
                    HttpContext.Current.Server.ClearError();
                    using (var errorController = new ErrorController())
                    {
                        ((IController)errorController).Execute(
                            new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData));
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = IoC.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
        }

        #endregion
    }
}