using System;
using System.Net;
using Castle.Core.Logging;
using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Providers;
using RestSharp;

namespace EdugameCloud.Lti.Desire2Learn
{
    public class Desire2LearnApiService : IDesire2LearnApiService
    {
        private readonly ILogger logger;
        private readonly ApplicationSettingsProvider settings;

        public string WhoAmIUrlFormat { get { return "/d2l/api/lp/{0}/users/whoami"; } }
        public string GetUserUrlFormat { get { return "/d2l/api/lp/{0}/users/{1}"; } }
        public string EnrollmentsUrlFormat { get { return "/d2l/api/lp/{0}/enrollments/orgUnits/{1}/users/"; } }
        public string EnrollmentsClasslistUrlFormat { get { return "/d2l/api/le/{0}/{1}/classlist/"; } }


        public Desire2LearnApiService(ILogger logger, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl, LmsCompany company)
        {
            var m_valenceHost = new HostSpec("https", hostUrl, 443);
            var appSettings = GetAppSettings(company);
            var context = InitializeAppContext(appSettings);
            return context.CreateUrlForAuthentication(m_valenceHost, returnUrl);
        }

        public T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl, LmsCompany company) where T : new()
        {
            var appSettings = GetAppSettings(company);
            var userContext = GetUserContext(appSettings, urlWithAuthParams, hostUrl);
            return MakeApiCall<T>(userContext, hostUrl, apiUrl);
        }

        public T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl, LmsCompany company) where T : new()
        {
            var appSettings = GetAppSettings(company);
            var userContext = GetUserContext(appSettings, userId, userKey, hostUrl);
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
                logger.InfoFormat("[D2L API call] Url: {0}, Status: {1}-{2}, ErrorMessage:{3}, Content:{4}",
                    apiUrl, (int) response.StatusCode, response.StatusDescription, response.ErrorMessage,
                    response.Content);
            }

            return response.Data;
        }

        private ID2LUserContext GetUserContext(Tuple<string, string> appSettings, Uri url, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(appSettings);
            return context.CreateUserContext(url, host);
        }

        private ID2LUserContext GetUserContext(Tuple<string, string> appSettings, string userId, string userKey, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(appSettings);
            return context.CreateUserContext(userId, userKey, host);
        }

        private ID2LAppContext InitializeAppContext(Tuple<string, string> appSettings)
        {
            if (string.IsNullOrEmpty(appSettings.Item1) || string.IsNullOrEmpty(appSettings.Item2))
            {
                throw new InvalidOperationException("D2L keys are not set"); // todo: log lmsCompanyId
            }

            return new D2LAppContextFactory().Create(appSettings.Item1, appSettings.Item2);
        }

        private Tuple<string, string> GetAppSettings(LmsCompany lmsCompany)
        {
            string appId = null;
            string appKey = null;
            var isSandbox = lmsCompany.GetSetting<bool>(LmsCompanySettingNames.IsD2LSandbox);
            if (isSandbox)
            {
                appId = lmsCompany.GetSetting<string>(LmsCompanySettingNames.D2LAppId);
                appKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.D2LAppKey);
                return new Tuple<string, string>(appId, appKey);
            }
            else
            {
                appId = ((dynamic) settings).D2LApiKey;
                appKey = ((dynamic)settings).D2LApiSecret;
            }

            return new Tuple<string, string>(appId, appKey);
        }
    }
}