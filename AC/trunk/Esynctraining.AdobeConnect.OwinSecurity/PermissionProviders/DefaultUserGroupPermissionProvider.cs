using System;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders
{
    public class DefaultUserGroupPermissionProvider : IUserGroupPermissionProvider
    {
        protected bool UserIsSpecialACGroupParticipant(AdobeConnectProvider provider, UserInfo user, PrincipalType groupType)
        {
            var enumGroupType = (PrincipalType)Enum.Parse(typeof(PrincipalType), groupType);
            if (enumGroupType == null) 
                throw new InvalidOperationException("Can't parse groupType, should be convertable to PrincipalType");
            var group = provider.GetGroupsByType(enumGroupType);
            if (group.Status.Code != StatusCodes.ok)
                return false;

            return UserIsGroupParticipant(provider, user, group.Values.First());
        }

        protected bool UserIsGroupParticipant(AdobeConnectProvider provider, UserInfo user, Principal groupPrincipal)
        {
            var participants = provider.GetGroupPrincipalUsers(groupPrincipal.PrincipalId, user.UserId);
            return participants.Success && participants.Values.Any(x => x.Login == user.Login);
        }

        public virtual bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            return UserIsSpecialACGroupParticipant(provider, user, PrincipalType.admins)
                || UserIsSpecialACGroupParticipant(provider, user, PrincipalType.live_admins);
        }

    }

}