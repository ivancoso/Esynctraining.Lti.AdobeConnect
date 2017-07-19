using System.Threading.Tasks;

namespace EdugameCloud.Lti.API.Schoology
{
    public interface ISchoologyRestApiClient
    {
        Task<T> GetRestCall<T>(string clientId, string clientSecret, string relativeUrl);

    }

}
