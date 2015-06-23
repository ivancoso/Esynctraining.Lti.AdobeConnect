using System;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API.Desire2Learn
{
    public interface IDesire2LearnApiService
    {
        string WhoAmIUrlFormat { get; }
        string GetUserUrlFormat { get; }
        string EnrollmentsUrlFormat { get; }
        string EnrollmentsClasslistUrlFormat { get; }

        Uri GetTokenRedirectUrl(Uri returnUrl, string hostUrl, LmsCompany company);
        T GetApiObjects<T>(Uri urlWithAuthParams, string hostUrl, string apiUrl, LmsCompany company) where T : new();
        T GetApiObjects<T>(string userId, string userKey, string hostUrl, string apiUrl, LmsCompany company) where T : new();

    }

}