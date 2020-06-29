using System;
using System.Web.Mvc;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.MVC.Attributes
{
    public class NonDebugModeRequireHttpsAttribute : RequireHttpsAttribute
    {
        private static readonly bool _isDebug;


        static NonDebugModeRequireHttpsAttribute()
        {
            dynamic settings = IoC.Resolve<ApplicationSettingsProvider>();
                bool val;
            _isDebug = bool.TryParse(settings.IsDebug, out val) && val;
        }

        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            if (_isDebug)
                return;

            // The base only redirects GET, but we added HEAD as well. This avoids exceptions for bots crawling using HEAD.
            // The other requests will throw an exception to ensure the correct verbs are used. 
            // We fall back to the base method as the mvc exceptions are marked as internal. 

            if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(filterContext.HttpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                base.HandleNonHttpsRequest(filterContext);
            }

            // Redirect to HTTPS version of page
            // We updated this to redirect using 301 (permanent) instead of 302 (temporary).
            string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;

            if (string.Equals(filterContext.HttpContext.Request.Url.Host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                // For localhost requests, default to IISExpress https default port (44300)
                url = "https://" + filterContext.HttpContext.Request.Url.Host + ":44300" + filterContext.HttpContext.Request.RawUrl;
            }

            filterContext.Result = new RedirectResult(url, false);
        }
    }

}
