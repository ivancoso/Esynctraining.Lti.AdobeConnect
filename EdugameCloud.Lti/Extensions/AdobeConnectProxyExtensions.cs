using System;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.Extensions
{
    public static class AdobeConnectProxyExtensions
    {
        public static int GetPermissionChunk(this IAdobeConnectProxy provider)
        {
            return 50;
        }

        public static StatusInfo UpdateScoPermissionForPrincipal(this IAdobeConnectProxy provider, string meetingScoId, string userPrincipalId, MeetingPermissionId permission)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));
            if (string.IsNullOrWhiteSpace(userPrincipalId))
                throw new ArgumentException("Non-empty value expected", nameof(userPrincipalId));

            return provider.UpdateScoPermissions(new MeetingPermissionUpdateTrio[]
            {
                new MeetingPermissionUpdateTrio
                {
                    ScoId = meetingScoId,
                    PrincipalId = userPrincipalId,
                    PermissionId = permission,
                }
            });
        }

    }

}
