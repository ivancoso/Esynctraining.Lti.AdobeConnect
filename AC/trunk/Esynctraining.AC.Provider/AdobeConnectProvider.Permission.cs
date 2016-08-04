namespace Esynctraining.AC.Provider
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The adobe connect provider.
    /// </summary>
    public partial class AdobeConnectProvider
    {
        /// <summary>
        /// The get SCO permissions for 'public-access'.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetScoPublicAccessPermissions(string scoId)
        {
            return this.GetPermissionsInfo(scoId, string.Format(CommandParams.PrincipalByPrincipalId, CommandParams.PrincipalIdPublicAccess));
        }

        /// <summary>
        /// Returns permissions for SCO (SCOs other than meetings or courses, e.g. files\folders)
        /// Returns only records with view\publish\manage\denied permissions.
        /// </summary>
        public PermissionCollectionResult GetScoPermissions(string scoId)
        {
            return this.GetPermissionsInfo(scoId, CommandParams.Permissions.Filter.PermissionId.NonMeetingAll);
        }

        public PermissionCollectionResult GetScoPermissions(string scoId, string principalId)
        {
            return this.GetPermissionsInfo(scoId, principalId);
        }

        /// The server defines a special principal, public-access, which combines with values of permission-id to create special access permissions to meetings.
        /// </summary>
        /// <param name="aclId">ACL id - required.</param>
        /// <param name="permissionId">Permission id - required.</param>
        /// <returns>Status Info.</returns>
        public StatusInfo UpdatePublicAccessPermissions(string aclId, SpecialPermissionId permissionId)
        {
            switch (permissionId)
            {
                case SpecialPermissionId.denied:
                    return this.UpdatePermissionsInternal(aclId, CommandParams.PrincipalIdPublicAccess, PermissionId.denied);
                case SpecialPermissionId.remove:
                    return this.UpdatePermissionsInternal(aclId, CommandParams.PrincipalIdPublicAccess, PermissionId.remove);
                case SpecialPermissionId.view_hidden:
                    return this.UpdatePermissionsInternal(aclId, CommandParams.PrincipalIdPublicAccess, PermissionId.view_hidden);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The update public access permissions.
        /// </summary>
        /// <param name="aclId">
        /// The acl id.
        /// </param>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo UpdatePublicAccessPermissions(string aclId, PermissionId permissionId)
        {
            return this.UpdatePermissionsInternal(aclId, CommandParams.PrincipalIdPublicAccess, permissionId);
        }

        public StatusInfo UpdateScoPermissions(IEnumerable<IPermissionUpdateTrio> values)
        {
            return this.UpdatePermissionsInternal(values);
        }
        
    }

}
