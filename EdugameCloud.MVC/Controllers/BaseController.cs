namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using Esynctraining.Core.Comparers;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    public abstract partial class BaseController : Controller
    {
        #region Constructors and Destructors
        
        protected BaseController(ApplicationSettingsProvider settings)
        {
            this.Settings = settings;
        }

        #endregion

        #region Public Properties
        
        public virtual string ActionName
        {
            get
            {
                return this.RouteData.With(x => x.Values["action"].ToString());
            }
        }
        
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
        
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            IT4MVCActionResult callInfo = result.GetT4MVCResult();
            return this.RedirectToRoute(callInfo.RouteValueDictionary);
        }

        #endregion
    }
}