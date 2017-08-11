using System.Configuration;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;
using System;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;

namespace Esynctraining.AdobeConnect.Security.Abstractions.PermissionProviders
{
    public class ConfigurationUserGroupPermissionProvider : DefaultUserGroupPermissionProvider
    {
        private string _groups;
        private string _domain;
        private string _login;
        private string _password;


        public ConfigurationUserGroupPermissionProvider(string groups, string domain, string user, string password)
        {
            _groups = groups;
            _domain = domain;
            _login = user;
            _password = password;
        }

        public override bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            var connectionDetails = new ConnectionDetails(new Uri(_domain));
            var providerAdm = new AdobeConnectProvider(connectionDetails);
            providerAdm.Logout();

            LoginResult result = providerAdm.Login(new UserCredentials(_login, _password));
            if (!result.Success)
            {
                return false;
            }

            if (UserIsSpecialACGroupParticipant(providerAdm, user, PrincipalType.admins))
                return true;

            if (string.IsNullOrEmpty(_groups))
                return false;

            string[] configGroups = _groups.Split(',', ';');
            var groups = providerAdm.GetGroupsByType(PrincipalType.group);

            if (groups?.Values != null)
            {
                var filteredGroups = groups.Values.Where(x => configGroups.Any(tg => tg.Trim() == x.Name));
                foreach (var groupPrincipal in filteredGroups)
                {
                    if (UserIsGroupParticipant(providerAdm, user, groupPrincipal))
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