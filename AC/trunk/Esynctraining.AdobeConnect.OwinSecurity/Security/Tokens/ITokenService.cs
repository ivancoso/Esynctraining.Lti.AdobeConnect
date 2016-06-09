using System;
using System.Threading.Tasks;

namespace Esynctraining.AdobeConnect.OwinSecurity.Security.Tokens
{
    public interface ITokenService
    {
        Task<bool> SaveTokenAsync(string principalId, string companyToken, string domain, Guid token, DateTime expiresUtc, string protectedTicket);
        Task<string> GetProtectedTicket(Guid token);
        Task Delete(Guid token);
    }
}
