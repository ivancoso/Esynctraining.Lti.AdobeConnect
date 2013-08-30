namespace eSyncTraining.Web.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects.Results;

    /// <summary>
    /// The adobe connected controller base.
    /// </summary>
    public abstract class AdobeConnectedControllerBase : Controller
    {
        /// <summary>
        /// The adobe connect provider.
        /// </summary>
        private AdobeConnectProvider adobeConnectProvider;

        /// <summary>
        /// Gets the adobe connect.
        /// </summary>
        protected AdobeConnectProvider AdobeConnect
        {
            get
            {
                return this.adobeConnectProvider ?? (this.adobeConnectProvider = new AdobeConnectProvider());
            }
        }

        /// <summary>
        /// The login with session.
        /// </summary>
        /// <returns>
        /// The <see cref="LoginResult"/>.
        /// </returns>
        protected LoginResult LoginWithSession()
        {
            var cookie = this.HttpContext.Request.Cookies[AdobeConnectProviderConstants.SessionCookieName];

            if (cookie == null)
            {
                return new LoginResult(null);
            }

            var result = this.AdobeConnect.LoginWithSessionId(cookie.Value);

            if (result.Success)
            {
                this.CreateSessionCookie(cookie.Value);
            }

            return result;
        }

        #region Helpers

        /// <summary>
        /// Gets the referrer url.
        /// </summary>
        protected string ReferrerUrl
        {
            get
            {
                return this.Request.UrlReferrer != null ? this.Request.UrlReferrer.PathAndQuery : null;
            }
        }

        /// <summary>
        /// The redirect to local.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        protected void CreateSessionCookie(string sessionId)
        {
            this.AddSessionCookie(sessionId, DateTime.Now.AddMinutes(29));
        }

        protected void DeleteSessionCookie()
        {
            this.AddSessionCookie(null, DateTime.Today.AddDays(-1));
        }

        private void AddSessionCookie(string sessionId, DateTime date)
        {
            var cookie = new HttpCookie(AdobeConnectProviderConstants.SessionCookieName, sessionId)
            {
                Expires = date
            };

            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        }

        #endregion
    }
}
