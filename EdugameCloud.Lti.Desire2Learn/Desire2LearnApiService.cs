using System;
using Castle.Core.Logging;
using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using RestSharp;

namespace EdugameCloud.Lti.Desire2Learn
{
    public class Desire2LearnApiService : IDesire2LearnApiService
    {
        private readonly ILogger logger;

        public string WhoAmIUrlFormat { get { return "/d2l/api/lp/{0}/users/whoami"; } }
        public string GetUserUrlFormat { get { return "/d2l/api/lp/{0}/users/{1}"; } }
        public string EnrollmentsUrlFormat { get { return "/d2l/api/lp/{0}/enrollments/orgUnits/{1}/users/"; } }
        public string EnrollmentsClasslistUrlFormat { get { return "/d2l/api/le/{0}/{1}/classlist/"; } }


        public Desire2LearnApiService(ILogger logger)
        {
            this.logger = logger;
        }

        public Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl, LmsCompany company)
        {
            var m_valenceHost = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppId), 
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppKey));
            return context.CreateUrlForAuthentication(m_valenceHost, returnUrl);
        }

        public T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl, LmsCompany company) where T : new()
        {
            var userContext = GetUserContext(
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppId), 
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppKey),
                urlWithAuthParams, 
                hostUrl);
            return MakeApiCall<T>(userContext, hostUrl, apiUrl);
        }

        public T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl, LmsCompany company) where T : new()
        {
            var userContext = GetUserContext(
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppId), 
                company.GetSetting<string>(LmsCompanySettingNames.D2LAppKey),
                userId, userKey, hostUrl);
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
            logger.InfoFormat("[D2L API call] Url: {0}, Status: {1}-{2}, ErrorMessage:{3}, Content:{4}", 
                apiUrl, (int)response.StatusCode, response.StatusDescription, response.ErrorMessage, response.Content);
            return response.Data;
        }

        private ID2LUserContext GetUserContext(string appId, string appKey, Uri url, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(appId, appKey);
            return context.CreateUserContext(url, host);
        }

        private ID2LUserContext GetUserContext(string appId, string appKey, string userId, string userKey, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            var context = InitializeAppContext(appId, appKey);
            return context.CreateUserContext(userId, userKey, host);
        }

        private ID2LAppContext InitializeAppContext(string appId, string appKey)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey))
            {
                throw new InvalidOperationException("D2L keys are not set"); // todo: log lmsCompanyId
            }
            return new D2LAppContextFactory().Create(appId, appKey);
        }
    }
}