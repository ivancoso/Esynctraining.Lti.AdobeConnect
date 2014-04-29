namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Web.Mvc;

    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.ViewModels;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    using Microsoft.Web.WebPages.OAuth;

    /// <summary>
    ///     The social controller.
    /// </summary>
    [HandleError]
    public partial class SocialController : BaseController
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialController"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings
        /// </param>
        public SocialController(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("callback")]
        [AllowAnonymous]
        public virtual ActionResult AuthenticationCallback(string provider)
        {
            string error;
            try
            {
                var result = OAuthWebSecurity.VerifyAuthentication();
                if (result.IsSuccessful)
                {
                    // name of the provider we just used
                    provider = provider ?? result.Provider;

                    // dictionary of values from identity provider
                    var extra = result.ExtraData;

                    if (extra.ContainsKey("accesstoken"))
                    {
                        return this.View(
                            EdugameCloudT4.Social.Views.LoggedIn,
                            new SocialViewModel
                            {
                                AccessToken = extra["accesstoken"],
                                Provider = provider,
                                BuildUrl = Links.Content.swf.pub.Url("SocialAccessProxy.swf"),
                                UserName = extra.ContainsKey("name") && !string.IsNullOrWhiteSpace(extra["name"]) ? extra["name"] : result.UserName,
                                AccessTokenSecret = this.Request.Params.HasKey("OAuthTokenSecret") ? Request.Params["OAuthTokenSecret"] : string.Empty
                            });
                    }
                }

                error = result.Error.Return(x => x.ToString(), "Generic fail");
            }
            catch (ApplicationException ex)
            {
                error = ex.Message;
            }

            return new ContentResult { Content = error };
        }

        /// <summary>
        /// The login with provider.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        [ActionName("login")]
        [HttpGet]
        public void LoginWithProvider(string provider)
        {
            OAuthWebSecurity.RequestAuthentication(provider, Url.AbsoluteAction(EdugameCloudT4.Social.AuthenticationCallback(provider)));
        }

        #endregion
    }
}