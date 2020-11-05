using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders
{
    public interface IUserGroupPermissionProvider
    {
        bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user);
    }
}