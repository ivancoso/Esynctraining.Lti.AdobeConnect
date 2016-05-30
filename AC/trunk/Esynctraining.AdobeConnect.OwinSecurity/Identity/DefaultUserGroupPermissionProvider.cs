using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public class DefaultUserGroupPermissionProvider : IUserGroupPermissionProvider
    {
        protected bool UserIsSpecialACGroupParticipant(AdobeConnectProvider provider, UserInfo user, string groupType)
        {
            var group = provider.GetGroupsByType(groupType);
            if (group.Item1.Code != StatusCodes.ok)
                return false;

            return UserIsGroupParticipant(provider, user, group.Item2.First());
        }

        protected bool UserIsGroupParticipant(AdobeConnectProvider provider, UserInfo user, Principal groupPrincipal)
        {
            var participants = provider.GetGroupPrincipalUsers(groupPrincipal.PrincipalId, user.UserId);
            return participants.Success && participants.Values.Any(x => x.Login == user.Login);
        }

        public virtual bool UserHasGroupPermission(AdobeConnectProvider provider, UserInfo user)
        {
            return UserIsSpecialACGroupParticipant(provider, user, "admins")
                   || UserIsSpecialACGroupParticipant(provider, user, "live-admins");
        }
    }
}