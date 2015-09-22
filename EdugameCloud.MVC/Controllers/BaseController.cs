namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using Esynctraining.Core.Comparers;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    ///     The base controller.
    /// </summary>
    public abstract partial class BaseController : Controller
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings
        /// </param>
        protected BaseController(ApplicationSettingsProvider settings)
        {
            this.Settings = settings;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the action name.
        /// </summary>
        public virtual string ActionName
        {
            get
            {
                return this.RouteData.With(x => x.Values["action"].ToString());
            }
        }

        /// <summary>
        /// Gets the controller name.
        /// </summary>
        public virtual string ControllerName
        {
            get
            {
                return this.RouteData.With(x => x.Values["controller"].ToString());
            }
        }

        /// <summary>
        ///   Gets the settings.
        /// </summary>
        public dynamic Settings { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get culture info from string.
        /// </summary>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        public CultureInfo GetCultureInfoFromString(string lang)
        {
            try
            {
                return CultureInfo.CreateSpecificCulture(lang);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The process version.
        /// </summary>
        /// <param name="swfFolder">
        /// The SWF folder.
        /// </param>
        /// <param name="buildSelector">
        /// The build Selector.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        protected string ProcessVersion(string swfFolder, string buildSelector)
        {
            var folder = this.Server.MapPath(swfFolder);
            if (Directory.Exists(folder))
            {
                var versions = new List<KeyValuePair<Version, string>>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var file in Directory.GetFiles(folder, buildSelector))
                {
                    var fileName = Path.GetFileName(file);
                    var version = fileName.GetBuildVersion();
                    versions.Add(new KeyValuePair<Version, string>(version, fileName));
                }

                versions.Sort(new BuildVersionComparer());
                return versions.FirstOrDefault().With(x => x.Value);
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The redirect to action.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="RedirectToRouteResult"/>.
        /// </returns>
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            IT4MVCActionResult callInfo = result.GetT4MVCResult();
            return this.RedirectToRoute(callInfo.RouteValueDictionary);
        }

        #endregion
    }
}