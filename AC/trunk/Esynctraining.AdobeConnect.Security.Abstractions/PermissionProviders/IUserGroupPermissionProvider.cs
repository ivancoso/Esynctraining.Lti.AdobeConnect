using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Security.Abstractions.PermissionProviders
{
    public interface IUserGroupPermissionProvider
    {
        bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user);

    }

}