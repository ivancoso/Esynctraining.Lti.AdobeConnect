namespace EdugameCloud.SocialStream.Host.Controllers
{
    using System.Web.Mvc;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    public abstract partial class BaseController : Controller
    {
        protected BaseController(ApplicationSettingsProvider settings)
        {
            this.Settings = settings;
        }

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

        public dynamic Settings { get; private set; }

    }

}