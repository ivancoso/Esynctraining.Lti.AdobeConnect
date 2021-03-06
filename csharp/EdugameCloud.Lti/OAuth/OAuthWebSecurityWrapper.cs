﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web;
using DotNetOpenAuth.AspNet;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.OAuth.Canvas;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.Constants;
using Microsoft.Web.WebPages.OAuth;

namespace EdugameCloud.Lti.OAuth
{
    /// <summary>
    /// The OAUTH web security wrapper.
    /// </summary>
    public static class OAuthWebSecurityWrapper
    {
        public static AuthenticationResult VerifyLtiAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret)
        {
            var httpClientFactory = IoC.Resolve<IHttpClientFactory>();
            var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value, httpClientFactory);
            return new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).VerifyAuthentication(null);
        }

        public static void RequestAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret, string returnUrl)
        {
            var httpClientFactory = IoC.Resolve<IHttpClientFactory>();
            var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value, httpClientFactory);
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
            var isSandbox = lmsCompany.GetSetting<bool>(LmsLicenseSettingNames.IsOAuthSandbox);
            if (isSandbox)
            {
                appId = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.OAuthAppId);
                appKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.OAuthAppKey);
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