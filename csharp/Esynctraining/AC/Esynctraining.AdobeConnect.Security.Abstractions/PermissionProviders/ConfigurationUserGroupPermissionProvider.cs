using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Security.Abstractions.PermissionProviders
{
    public class ConfigurationUserGroupPermissionProvider : DefaultUserGroupPermissionProvider
    {
        private string[] _configGroups;

        public ConfigurationUserGroupPermissionProvider(string[] configGroups)
        {
            _configGroups = configGroups;
        }

        public override bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            if (UserIsSpecialACGroupParticipant(provider, user, PrincipalType.admins))
                return true;

            if (!_configGroups.Any())
                return false;

            var groups = provider.GetGroupsByType(PrincipalType.group);

            if (groups?.Values != null)
            {
                var filteredGroups = groups.Values.Where(x => _configGroups.Any(tg => tg.Trim() == x.Name));
                foreach (var groupPrincipal in filteredGroups)
                {
                    if (UserIsGroupParticipant(provider, user, groupPrincipal))
                        return true;
                }
            }

            return false;
        }
    }
}