using System.Collections.Generic;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.OAuth
{
    using System;
    using System.Reflection;
    using System.Web;

    using DotNetOpenAuth.AspNet;

    using EdugameCloud.Lti.OAuth.Canvas;

    using Esynctraining.Core.Providers;

    using Microsoft.Web.WebPages.OAuth;

    /// <summary>
    /// The OAUTH web security wrapper.
    /// </summary>
    public static class OAuthWebSecurityWrapper
    {
        public static AuthenticationResult VerifyLtiAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret)
        {
            var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value);
            return new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).VerifyAuthentication(null);
        }

        public static void RequestAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret, string returnUrl)
        {
            var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value);
            new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).RequestAuthentication(returnUrl);
        }

        private static IOpenAuthDataProvider GetProvider(Type type)
        {
            var field = type.GetField("OAuthDataProvider", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                return (IOpenAuthDataProvider)field.GetValue(null);
            }
            else
            {
                return null;
            }
        }

        //todo: move to companyLms/settings service?
        public static KeyValuePair<string, string> GetOAuthSettings(ILmsLicense lmsCompany, string globalAppId, string globalAppKey)
        {
            string appId = null;
            string appKey = null;
            var isSandbox = lmsCompany.GetSetting<bool>(LmsCompanySettingNames.IsOAuthSandbox);
            if (isSandbox)
            {
                appId = lmsCompany.GetSetting<string>(LmsCompanySettingNames.OAuthAppId);
                appKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.OAuthAppKey);
            }
            else
            {
                appId = globalAppId;
                appKey = globalAppKey;
            }

            return new KeyValuePair<string, string>(appId, appKey);
        }


    }
}