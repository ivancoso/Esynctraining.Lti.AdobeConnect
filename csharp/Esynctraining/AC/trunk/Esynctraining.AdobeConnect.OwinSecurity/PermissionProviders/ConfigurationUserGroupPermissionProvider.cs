using System.Configuration;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders
{
    public class ConfigurationUserGroupPermissionProvider : DefaultUserGroupPermissionProvider
    {
        public override bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            if (UserIsSpecialACGroupParticipant(provider, user, PrincipalType.admins))
                return true;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MonitoringGroups"]))
                return false;

            string[] configGroups = ConfigurationManager.AppSettings["MonitoringGroups"].Split(',', ';');
            var groups = provider.GetGroupsByType(PrincipalType.group);

            if (groups?.Values != null)
            {
                var filteredGroups = groups.Values.Where(x => configGroups.Any(tg => tg.Trim() == x.Name));
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