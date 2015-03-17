using System;
using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;
using EdugameCloud.Lti.Constants;
using RestSharp;

namespace EdugameCloud.Lti.API.Desire2Learn
{

    public interface IDesire2LearnApiService
    {
        Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl);
        T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl) where T : new();
        T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl) where T : new();
    }

    public class Desire2LearnApiService : IDesire2LearnApiService//,ILmsApi
    {
        public const string ApiVersion = "1.4";
        public const string WhoAmIUrlFormat = "/d2l/api/lp/{0}/users/whoami";
        public const string GetUserUrlFormat = "/d2l/api/lp/{0}/users/{1}";
        public const string EnrollmentsUrlFormat = "/d2l/api/lp/{0}/enrollments/orgUnits/{1}/users/";

        private readonly string appId;
        private readonly string appSecret;
        private readonly ID2LAppContext appContext;

        public Desire2LearnApiService(string providerKey, string providerSecret)
        {
            appId = providerKey;
            appSecret = providerSecret;
            appContext = new D2LAppContextFactory().Create(appId, appSecret);
        }

        public Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl)
        {
            var m_valenceHost = new HostSpec("https", hostUrl, 443);
            return appContext.CreateUrlForAuthentication(m_valenceHost, returnUrl);
        }

        public T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl) where T : new()
        {
            var userContext = GetUserContext(urlWithAuthParams, hostUrl);
            return MakeApiCall<T>(userContext, hostUrl, apiUrl);
        }

        public T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl) where T : new()
        {
            var userContext = GetUserContext(userId, userKey, hostUrl);
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
            return response.Data;
        }

        private ID2LUserContext GetUserContext(Uri url, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            return appContext.CreateUserContext(url, host);
        }

        private ID2LUserContext GetUserContext(string userId, string userKey, string hostUrl)
        {
            var host = new HostSpec("https", hostUrl, 443);
            return appContext.CreateUserContext(userId, userKey, host);
        }
    }
}