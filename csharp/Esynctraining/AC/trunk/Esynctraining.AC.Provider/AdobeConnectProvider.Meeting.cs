namespace Esynctraining.AC.Provider
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.EntityParsing;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The adobe connect provider.
    /// </summary>
    public partial class AdobeConnectProvider
    {
        /// <summary>
        /// The get meetings by SCO id.
        /// </summary>
        /// <param name="folderScoId">
        /// The folder SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetMeetingsByFolder(string folderScoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.Meetings, folderScoId), out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), folderScoId)
                : new ScoContentCollectionResult(status);
        }


        //public PermissionCollectionResult GetMeetingHosts(string meetingId)
        //{
        //    return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.Host);
        //}

        //public PermissionCollectionResult GetMeetingPresenters(string meetingId)
        //{
        //    return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.MiniHost);
        //}

        //public PermissionCollectionResult GetMeetingParticipants(string meetingId)
        //{
        //    return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.View);
        //}

        /// <summary>
        /// Returns ALL meeting participants (hosts, presenters and participans)
        /// </summary>

        public PermissionCollectionResult GetScoPermissionsByPermissionId(string scoId, IEnumerable<PermissionId> permissionsFilter)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("SCO can't be empty", nameof(scoId));
            if (permissionsFilter == null)
                throw new ArgumentNullException(nameof(scoId));

            var filter = string.Join("&",
                permissionsFilter.Select(
                    x =>
                        string.Format(CommandParams.Permissions.Filter.PermissionId.Format,
                            x.GetACEnum())));

            return GetPermissionsInfo(scoId, filter);
        }

        public MeetingPermissionCollectionResult GetAllMeetingEnrollments(string meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting SCO can't be empty", nameof(meetingId));

            PermissionCollectionResult result = null;

            result = GetPermissionsInfo(meetingId, CommandParams.Permissions.Filter.PermissionId.MeetingAll);
            
            // ACLTI-2307 - workaround. Need to remove when migrated to HttpClient. WebResponse throws IOException (unexpected EOF) in some cases.
            if (!result.Success && result.Status.UnderlyingExceptionInfo is IOException)
            { 
                int i = 0;
                int chunk = 30; //for 30 records server does not return header Transfer-Encoding: chunked
                PermissionCollectionResult res;
                List<PermissionInfo> permissions = new List<PermissionInfo>();
                do
                {
                    // action=permissions-info
                    var commandParams = string.Format(CommandParams.Permissions.AclId + "&{1}", meetingId, CommandParams.Permissions.Filter.PermissionId.MeetingAll);

                    res = DoCallPermissionsInfo(Commands.Permissions.Info, commandParams, chunk * i++, chunk);
                    if(res.Success)
                    {
                        permissions.AddRange(res.Values);
                    }
                }
                while (res.Values.Any());

                var meetingPermissions = permissions.Select(x => new MeetingPermissionInfo(x)).ToList();
                return new MeetingPermissionCollectionResult(res.Status, meetingPermissions);
            }

            return result.ConvertForMeeting();
        }

        public MeetingPermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting SCO can't be empty", nameof(meetingId));
            if (principalIds == null)
                throw new ArgumentNullException(nameof(principalIds));

            var filter = new StringBuilder(23 * principalIds.Count());
            foreach (string principalId in principalIds)
                filter.AppendFormat("&" + CommandParams.PrincipalByPrincipalId, principalId);

            return this.GetPermissionsInfo(meetingId, filter.ToString()).ConvertForMeeting();
        }

        public StatusInfo UpdateVirtualClassroomLicenseModel(string scoId, bool enableNamedVcLicenseModel)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.VirtualClassroom.VirtualClassroomLicenseModelUpdate, 
                string.Format(CommandParams.VirtualClassroom.LicenseModelUpdate, scoId, enableNamedVcLicenseModel.ToString().ToLower()), out status);

            return status;
        }

    }

}
