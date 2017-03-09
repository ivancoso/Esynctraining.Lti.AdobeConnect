using System.Web.Mvc;
using EdugameCloud.Lti.API;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Providers;

namespace EdugameCloud.MVC.Controllers
{
    public abstract partial class BaseController : Controller
    {
        #region Constructors and Destructors
        
        protected BaseController(ApplicationSettingsProvider settings)
        {
            Settings = settings;
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


        #region Methods
        
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            IT4MVCActionResult callInfo = result.GetT4MVCResult();
            return this.RedirectToRoute(callInfo.RouteValueDictionary);
        }

        #endregion
    }
}