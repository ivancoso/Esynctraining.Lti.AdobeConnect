using System;
using System.Collections.Generic;
using System.Net;
using D2L.Extensibility.AuthSdk;
using EdugameCloud.Lti.API.Desire2Learn;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using RestSharp;
using HttpScheme = EdugameCloud.Lti.Core.Constants.HttpScheme;

namespace EdugameCloud.Lti.Desire2Learn
{
    public class Desire2LearnApiService : IDesire2LearnApiService
    {
        private readonly ILogger _logger;

        public string WhoAmIUrlFormat => "/d2l/api/lp/{0}/users/whoami";
        public string GetUserUrlFormat => "/d2l/api/lp/{0}/users/{1}";
        public string EnrollmentsUrlFormat => "/d2l/api/lp/{0}/enrollments/orgUnits/{1}/users/";
        public string EnrollmentsClasslistUrlFormat => "/d2l/api/le/{0}/{1}/classlist/";


        public Desire2LearnApiService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl, Dictionary<string, object> licenseSettings)
        {
            var m_valenceHost = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(licenseSettings);
            return context.CreateUrlForAuthentication(m_valenceHost, returnUrl);
        }

        public T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl, Dictionary<string, object> licenseSettings) where T : new()
        {
            var userContext = GetUserContext(licenseSettings, urlWithAuthParams, hostUrl);
            return MakeApiCall<T>(userContext, hostUrl, apiUrl);
        }

        public T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl, Dictionary<string, object> licenseSettings) where T : new()
        {
            var userContext = GetUserContext(licenseSettings, userId, userKey, hostUrl);
            return MakeApiCall<T>(userContext, hostUrl, apiUrl);
        }

        private T MakeApiCall<T>(ID2LUserContext userContext, string hostUrl, string apiUrl) where T : new()
        {
            if (userContext == null)
            {
                throw new Exception("This method can only be used for an authenticated user");
            }

            var client = new RestClient(HttpScheme.Https + hostUrl);
            var authenticator = new ValenceAuthenticator(userContext);
            var request = new RestRequest(apiUrl);
            authenticator.Authenticate(client, request);
            var response = client.Execute<T>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.InfoFormat("[D2L API call] Url: {0}, Status: {1}-{2}, ErrorMessage:{3}, Content:{4}",
                    apiUrl, (int)response.StatusCode, response.StatusDescription, response.ErrorMessage,
                    response.Content);
            }

            return response.Data;
        }

        private ID2LUserContext GetUserContext(Dictionary<string, object> licenseSettings, Uri url, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(licenseSettings);
            return context.CreateUserContext(url, host);
        }

        private ID2LUserContext GetUserContext(Dictionary<string, object> licenseSettings, string userId, string userKey, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(licenseSettings);
            return context.CreateUserContext(userId, userKey, host);
        }

        private ID2LAppContext InitializeAppContext(Dictionary<string, object> licenseSettings)
        {
            var appId = (string)licenseSettings[LmsLicenseSettingNames.BrightspaceAppId];
            var appKey = (string)licenseSettings[LmsLicenseSettingNames.BrightspaceAppKey];
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey))
            {
                throw new InvalidOperationException("D2L keys are not set"); // todo: log lmsCompanyId
            }

            return new D2LAppContextFactory().Create(appId, appKey);
        }
    }
}