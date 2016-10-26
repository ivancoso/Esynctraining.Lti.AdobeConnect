using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.MVC.Social.OAuth;
using EdugameCloud.MVC.Social.Subscriptions;
using EdugameCloud.SocialStream.Host.ViewModels;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.Web.WebPages.OAuth;

namespace EdugameCloud.SocialStream.Host.Controllers
{
    [HandleError]
    public partial class SocialController : BaseController
    {
        private readonly SocialUserTokensModel socialUserTokensModel;
        private readonly RealTimeNotificationModel realTimeNotificationModel;

        private readonly ILogger logger;


        public SocialController(SocialUserTokensModel socialUserTokensModel, RealTimeNotificationModel realTimeNotificationModel, ILogger logger, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.socialUserTokensModel = socialUserTokensModel;
            this.realTimeNotificationModel = realTimeNotificationModel;
            this.logger = logger;
        }


        [ActionName("login")]
        [HttpGet]
        public void LoginWithProvider(string provider, string key)
        {
            try
            {
                logger.Info($"LoginWithProvider Provider:{provider}. IP:{HttpContextExtensions.GetIPAddress()}");
                var login = Url.AbsoluteAction(EdugameCloudT4.Social.AuthenticationCallback(provider, key)) + "?key=" + key;
                OAuthWebSecurity.RequestAuthentication(provider, login);
            }
            catch (Exception ex)
            {
                logger.Error("LoginWithProvider", ex);
                throw;
            }
        }

        [ActionName("callback")]
        [AllowAnonymous]
        public virtual ActionResult AuthenticationCallback(string provider, string key)
        {
            string error;
            try
            {
                logger.Info($"AuthenticationCallback Provider:{provider}. IP:{HttpContextExtensions.GetIPAddress()}");

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
                logger.Error("AuthenticationCallback", ex);
                error = ex.Message;
            }

            return new ContentResult { Content = error };
        }

        [ActionName("realtime-callback")]
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult RealtimeCallback(string provider, InstagramSubscriptionHub hub)
        {
            logger.Info($"RealtimeCallback GET Provider:{provider}. IP:{HttpContextExtensions.GetIPAddress()}");

            return this.Content(hub.challenge);
        }

        [ActionName("realtime-callback")]
        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult RealtimeCallback(string provider)
        {
            logger.Info($"RealtimeCallback POST Provider:{provider}. IP:{HttpContextExtensions.GetIPAddress()}");

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


        private void InsertRealTimeCallbackData(string connectionString, IEnumerable<SubscriptionUpdateDTO> dtos, ILogger log)
        {
            const string InsertCommand = 
                @"declare @logCount int
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

    }

    public static class UrlExtensions
    {
        public static string AbsoluteAction(
            this UrlHelper url, string action, string controller, object roteValues = null, string schema = "http")
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            string absoluteAction;
            if (requestUrl != null)
            {
                absoluteAction = string.Format(
                    "{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, url.Action(action, controller, roteValues));
            }
            else
            {
                absoluteAction = url.Action(action, controller, roteValues, schema);
            }

            return absoluteAction;
        }

        public static string HttpActionAbsolute(this UrlHelper urlHelper, ActionResult result)
        {
            var portToUse = int.Parse((string)((dynamic)IoC.Resolve<ApplicationSettingsProvider>()).HttpPort);
            var builder = new UriBuilder(urlHelper.RequestContext.HttpContext.Request.Url) { Scheme = Uri.UriSchemeHttp, Port = portToUse };
            return string.Format("{0}{1}", builder.Uri.GetLeftPart(UriPartial.Authority), urlHelper.RouteUrl(result.GetRouteValueDictionary()));
        }
        
        public static string AbsoluteAction(this UrlHelper url, ActionResult actionResult, object roteValues = null, string schema = "http")
        {
            var callInfo = actionResult.GetT4MVCResult();
            return AbsoluteAction(url, callInfo.Action, callInfo.Controller, roteValues, schema);
        }

    }

    public static class HttpContextExtensions
    {
        #region Public Methods and Operators

        public static string GetIPAddress()
        {
            HttpContext context = HttpContext.Current;
            return context.GetIPAddress();
        }

        //public static string GetIPAddress(this HttpContext context)
        //{
        //    string address = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(address))
        //    {
        //        string[] addresses = address.Split(',');
        //        if (addresses.Length != 0)
        //        {
        //            return addresses[0];
        //        }
        //    }

        //    return context.Request.ServerVariables["REMOTE_ADDR"];
        //}

        #endregion
    }

}