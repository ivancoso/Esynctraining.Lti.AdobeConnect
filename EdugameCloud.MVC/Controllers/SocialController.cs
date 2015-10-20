namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Esynctraining.Core.Logging;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.Social.OAuth;
    using EdugameCloud.MVC.Social.Subscriptions;
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
        private readonly RealTimeNotificationModel realTimeNotificationModel;

        private readonly ILogger logger;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialController"/> class.
        /// </summary>
        /// <param name="socialUserTokensModel">
        /// The social User Tokens Model.
        /// </param>
        /// <param name="realTimeNotificationModel">
        /// The RTMP Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public SocialController(SocialUserTokensModel socialUserTokensModel, RealTimeNotificationModel realTimeNotificationModel, ILogger logger, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.socialUserTokensModel = socialUserTokensModel;
            this.realTimeNotificationModel = realTimeNotificationModel;
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="hub">
        /// The hub.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("realtime-callback")]
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult RealtimeCallback(string provider, InstagramSubscriptionHub hub)
        {
            return this.Content(hub.challenge);
        }

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="updates">
        /// The updates.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("realtime-callback")]
        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult RealtimeCallback(string provider)
        {
            var connectionString = (string)this.Settings.ConnectionString;
            var updates = new List<SubscriptionUpdateDTO>();
            string data = "Not received";
            var log = this.logger;
            try
            {
                data = System.Text.Encoding.ASCII.GetString(HttpContext.Request.InputStream.ReadToEnd());
                var des = (SubscriptionUpdateWrapper)Newtonsoft.Json.JsonConvert.DeserializeObject("{data:" + data + "}", typeof(SubscriptionUpdateWrapper));
                updates = des.data.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Social Realtime Error data=" + data, ex);
            }

            if (updates.Any() && bool.Parse((string)Settings.SocialSubscriptionsEnabled))
            {
                Task.Factory.StartNew(() => this.InsertRealTimeCallbackData(connectionString, updates, log));
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// The insert real time callback data.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="dtos">
        /// The DTOS.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        private void InsertRealTimeCallbackData(string connectionString, IEnumerable<SubscriptionUpdateDTO> dtos, ILogger log)
        {
            const string InsertCommand = @"declare @logCount int
		                                        set @logCount = 0
		                                        select @logCount = count(*) from [EduGameCloud].[dbo].[SubscriptionHistoryLog] where subscriptionId = @subscriptionId
		                                        if (@logCount > 0)
		                                        begin
                                                insert into [EduGameCloud].[dbo].[SubscriptionUpdate] ([subscription_id],[object],[object_id],[changed_aspect],[time],[createdDate]) values(@subscriptionId,@objectType,@objectId,@changedAspect,@time,GETDATE())
                                                end";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (var dto in dtos)
                {
                    try
                    {
                        using (var cmd = new SqlCommand(InsertCommand, con))
                        {
                            cmd.Parameters.AddWithValue("@subscriptionId", dto.subscription_id);
                            cmd.Parameters.AddWithValue("@objectType", dto.@object);
                            cmd.Parameters.AddWithValue("@objectId", dto.object_id);
                            cmd.Parameters.AddWithValue("@changedAspect", dto.changed_aspect);
                            cmd.Parameters.AddWithValue("@time", dto.time);
                            cmd.ExecuteNonQuery();
                        }
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception ex)
                    {
                        try
                        {
                            log.Error("Social subscription update. Failed to insert: " + dto, ex);
                        }
                            // ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                        }
                    }
                }
            }
        }

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
                                this.realTimeNotificationModel.NotifyClientsAboutSocialTokens(new SocialUserTokensDTO(tokens));
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