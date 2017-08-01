using System.Configuration;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Security.Abstractions.PermissionProviders
{
    public class ConfigurationUserGroupPermissionProvider : DefaultUserGroupPermissionProvider
    {
        private string _groups;

        public ConfigurationUserGroupPermissionProvider(string groups)
        {
            _groups = groups;
        }

        public override bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            if (UserIsSpecialACGroupParticipant(provider, user, PrincipalType.admins))
                return true;

            if (string.IsNullOrEmpty(_groups))
                return false;

            string[] configGroups = _groups.Split(',', ';');
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

            //if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MonitoringGroups"]))
            //    return false;

            //string[] configGroups = ConfigurationManager.AppSettings["MonitoringGroups"].Split(',', ';');
            //var groups = provider.GetGroupsByType(PrincipalType.group);

            //if (groups?.Values != null)
            //{
            //    var filteredGroups = groups.Values.Where(x => configGroups.Any(tg => tg.Trim() == x.Name));
            //    foreach (var groupPrincipal in filteredGroups)
            //    {
            //        if (UserIsGroupParticipant(provider, user, groupPrincipal))
            //            return true;
            //    }
            //}

            //return false;
        }
    }
}