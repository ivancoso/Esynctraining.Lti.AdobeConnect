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
            if (UserIsSpecialACGroupParticipant(provider, user, "admins"))
                return true;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MonitoringGroups"]))
                return false;

            string[] configGroups = ConfigurationManager.AppSettings["MonitoringGroups"].Split(',', ';');
            var groups = provider.GetGroupsByType("group");

            if (groups?.Item2 != null)
            {
                var filteredGroups = groups.Item2.Where(x => configGroups.Any(tg => tg.Trim() == x.Name));
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