using System;
using System.Collections.Generic;
using System.Text;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain.Entities;

namespace Esynctraining.Lti.Zoom.OAuth
{
    /// <summary>
    /// The OAUTH web security wrapper.
    /// </summary>
    public static class OAuthWebSecurityWrapper
    {
        //public static AuthenticationResult VerifyLtiAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret)
        //{
        //    var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value);
        //    return new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).VerifyAuthentication(null);
        //}

        //public static void RequestAuthentication(HttpContextBase context, KeyValuePair<string, string> appIdWithSecret, string returnUrl)
        //{
        //    var canvasClient = new CanvasClient(appIdWithSecret.Key, appIdWithSecret.Value);
        //    new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).RequestAuthentication(returnUrl);
        //}

        //private static IOpenAuthDataProvider GetProvider(Type type)
        //{
        //    var field = type.GetField("OAuthDataProvider", BindingFlags.Static | BindingFlags.NonPublic);
        //    if (field != null)
        //    {
        //        return (IOpenAuthDataProvider)field.GetValue(null);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //todo: move to companyLms/settings service?
        public static KeyValuePair<string, string> GetOAuthSettings(LmsLicenseDto lmsCompany, string globalAppId, string globalAppKey)
        {
            string appId = null;
            string appKey = null;
            //var isSandbox = lmsCompany.GetSetting<bool>(LmsLicenseSettingNames.IsOAuthSandbox);
            //if (isSandbox)
            //{
                appId = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                appKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthKey);
            //}
            //else
            //{
            //    appId = globalAppId;
            //    appKey = globalAppKey;
            //}

            return new KeyValuePair<string, string>(appId, appKey);
        }


    }
}
