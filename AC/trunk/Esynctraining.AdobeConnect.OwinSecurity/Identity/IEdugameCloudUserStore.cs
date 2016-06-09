using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public interface IEdugameCloudUserStore<T> : IUserStore<T> where T : AdobeConnectUser, new()
    {
        Task<T> FindByPrincipalIdAndCompanyTokenAndAcDomainAsync(string principalId, string companyToken, string acDomain);
        Task CreateAsync(T user, string password);
        Task<string> RetrievePassword(string principalId, string companyToken, string acDomain);
    }
}