namespace Esynctraining.AC.Provider
{
    using System;
    using System.Collections.Generic;
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
        public MeetingPermissionCollectionResult GetAllMeetingEnrollments(string meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting SCO can't be empty", nameof(meetingId));

            return this.GetPermissionsInfo(meetingId, CommandParams.Permissions.Filter.PermissionId.MeetingAll).ConvertForMeeting();
        }

        public MeetingPermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting SCO can't be empty", nameof(meetingId));
            if (principalIds == null)
                throw new ArgumentNullException("principalIds");

            var filter = new StringBuilder(23 * principalIds.Count());
            foreach (string principalId in principalIds)
                filter.AppendFormat("&" + CommandParams.PrincipalByPrincipalId, principalId);

            return this.GetPermissionsInfo(meetingId, filter.ToString()).ConvertForMeeting();
        }
 
    }

}
