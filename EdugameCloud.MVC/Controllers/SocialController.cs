namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Web.Mvc;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.OAuth;
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
        /// <summary>
        /// The social user tokens model.
        /// </summary>
        private readonly SocialUserTokensModel socialUserTokensModel;

        /// <summary>
        /// The RTMP model.
        /// </summary>
        private readonly RTMPModel rtmpModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialController"/> class.
        /// </summary>
        /// <param name="socialUserTokensModel">
        /// The social User Tokens Model.
        /// </param>
        /// <param name="rtmpModel">
        /// The RTMP Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public SocialController(SocialUserTokensModel socialUserTokensModel, RTMPModel rtmpModel, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.socialUserTokensModel = socialUserTokensModel;
            this.rtmpModel = rtmpModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("callback")]
        [AllowAnonymous]
        public virtual ActionResult AuthenticationCallback(string provider, string key)
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
                        var token = extra["accesstoken"];
                        var secret = provider == "twitter"
                                         ? TwitterClient2.TokenManager.GetTokenSecret(extra["accesstoken"])
                                         : string.Empty;

                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            var tokens = this.socialUserTokensModel.GetOneByKey(key).Value;
                            tokens = tokens ?? new SocialUserTokens();
                            tokens.Key = key;
                            tokens.Provider = provider;
                            tokens.Token = token;
                            tokens.Secret = secret;
                            this.socialUserTokensModel.RegisterSave(tokens, true);
                            try
                            {
                                this.rtmpModel.NotifyClientsAboutSocialTokens(new SocialUserTokensDTO(tokens));
                            }
                            catch (Exception ex)
                            {

                            }
                            
                        }

                        return this.View(
                            EdugameCloudT4.Social.Views.LoggedIn,
                            new SocialViewModel
                            {
                                AccessToken = token,
                                Provider = provider,
                                BuildUrl = Links.Content.swf.pub.Url("SocialAccessProxy.swf"),
                                UserName = extra.ContainsKey("name") && !string.IsNullOrWhiteSpace(extra["name"]) ? extra["name"] : result.UserName,
                                AccessTokenSecret = secret
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
        /// <param name="key">
        /// The key.
        /// </param>
        [ActionName("login")]
        [HttpGet]
        public void LoginWithProvider(string provider, string key)
        {
            var login = Url.AbsoluteAction(EdugameCloudT4.Social.AuthenticationCallback(provider, key)) + "?key=" + key;
            OAuthWebSecurity.RequestAuthentication(provider, login);
        }

        #endregion
    }
}