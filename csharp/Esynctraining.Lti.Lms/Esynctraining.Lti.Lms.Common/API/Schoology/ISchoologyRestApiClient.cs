using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Common.API.Schoology
{
    public interface ISchoologyRestApiClient
    {
        Task<T> GetRestCall<T>(string clientId, string clientSecret, string relativeUrl);

    }

}
