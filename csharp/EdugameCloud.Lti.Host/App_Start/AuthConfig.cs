using System.Collections.Generic;
using System.Net.Http;
using EdugameCloud.Lti.OAuth.Canvas;
using Microsoft.Web.WebPages.OAuth;

namespace EdugameCloud.Lti.Host
{
    public static class AuthConfig
    {
        public static void RegisterAuth(dynamic settings, IHttpClientFactory httpClientFactory)
        {
            //// To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            //// you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            string displayName = "Canvas";
            IDictionary<string, object> extraData = new Dictionary<string, object>();
            OAuthWebSecurity.RegisterClient(
                new CanvasClient((string)settings.CanvasClientId, (string)settings.CanvasClientSecret, httpClientFactory), 
                displayName, 
                extraData);
        }
    }
}
