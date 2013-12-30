namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Web.Mvc;

    using EdugameCloud.MVC.Wrappers;

    /// <summary>
    ///     The error controller.
    /// </summary>
    [HandleError]
    public partial class ErrorController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="exc">
        /// The exc.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Index(Exception exc)
        {
            return this.View(
                EdugameCloudT4.Shared.Views.Error,
                new HandleErrorInfoWrapper(exc, EdugameCloudT4.Error.Index()));
        }

        #endregion
    }
}