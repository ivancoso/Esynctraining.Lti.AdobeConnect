using System.Collections.Generic;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders
{
    public interface IUserAuthorizationProvider
    {
        IEnumerable<string> GetUserPermissions(AdobeConnectProvider provider, UserInfo user);
    }
}