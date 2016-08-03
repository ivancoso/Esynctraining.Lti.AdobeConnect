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
        #region Fields

        /// <summary>
        /// The request processor
        /// </summary>
        private readonly RequestProcessor requestProcessor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdobeConnectProvider"/> class.
        /// </summary>
        /// <param name="connectionDetails">
        /// The connection details.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// ConnectionDetails should not be null
        /// </exception>
        public AdobeConnectProvider(ConnectionDetails connectionDetails)
        {
            if (connectionDetails == null)
                throw new ArgumentNullException(nameof(connectionDetails));

            this.requestProcessor = new RequestProcessor(connectionDetails, null);
        }

        #endregion

        #region Login/Logout

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <returns>
        /// The <see cref="LoginResult"/>.
        /// </returns>
        public LoginResult Login(UserCredentials credentials)
        {
            this.requestProcessor.SetSessionId(null);

            StatusInfo statusInfo;
            var success = this.LoginInternal(credentials.Login, credentials.Password, out statusInfo);

            UserInfo user = null;
            if (success)
            {
                var commonInfo = GetCommonInfo();
                user = commonInfo.Success ? commonInfo.CommonInfo.User : null;
            }

            return new LoginResult(statusInfo, success ? user : null);
        }

        /// <summary>
        /// Logs the User in with session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// The <see cref="LoginResult"/>.
        /// </returns>
        public LoginResult LoginWithSessionId(string sessionId)
        {
            this.requestProcessor.SetSessionId(sessionId);

            StatusInfo status;
            var shortcuts = this.requestProcessor.Process(Commands.Sco.Shortcuts, null, out status);

            UserInfo user = null;
            if (status.Code == StatusCodes.ok)
            {
                var commonInfo = GetCommonInfo();
                if (commonInfo.Success)
                    user = commonInfo.CommonInfo.User;
            }

            return new LoginResult(status, user);
        }

        /// <summary>
        /// Performs log-out procedure
        /// </summary>
        public void Logout()
        {
            // action=logout
            StatusInfo status;

            this.requestProcessor.Process(Commands.Logout, null, out status);
        }

        ///// <summary>
        ///// Gets user info.
        ///// </summary>
        ///// <returns>
        ///// The <see cref="UserInfo"/>.
        ///// </returns>
        //public UserInfo GetUserInfo()
        //{
        //    StatusInfo status;

        //    return this.GetUserInfo(out status);
        //}

        ///// <summary>
        ///// Returns information about currently logged in user
        ///// </summary>
        ///// <param name="status">The status.</param>
        ///// <returns>
        /////   <see cref="UserInfo"/>
        ///// </returns>
        //public UserInfo GetUserInfo(out StatusInfo status)
        //{
        //    // action=common-info
        //    var doc = this.requestProcessor.Process(Commands.CommonInfo, null, out status);

        //    if (!ResponseIsOk(doc, status))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        return UserInfoParser.Parse(doc);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceTool.TraceException(ex);
        //    }

        //    return null;
        //}

        #endregion

        #region Read

        /// <summary>
        /// List all meetings on the server
        /// </summary>
        /// <returns>
        /// <see cref="MeetingItem">Meeting list</see>
        /// *Note: all dates are GMT
        /// </returns>
        public MeetingItemCollectionResult ReportAllMeetings()
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkObjects, CommandParams.ReportBulkObjectsFilters.Meeting, out status);

            return ResponseIsOk(doc, status)
                       ? new MeetingItemCollectionResult(status, MeetingItemCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl, null))
                       : new MeetingItemCollectionResult(status);
        }

        public MeetingItemCollectionResult ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-objects"
            StatusInfo status;
            string parameters = string.Format(CommandParams.ReportBulkObjectsFilters.MeetingByNameLike, nameLikeCriteria).AppendPagingIfNeeded(startIndex, limit);
            var doc = this.requestProcessor.Process(Commands.ReportBulkObjects, parameters, out status);

            return ResponseIsOk(doc, status)
                       ? new MeetingItemCollectionResult(status, MeetingItemCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl, null))
                       : new MeetingItemCollectionResult(status);
        }

        /// <summary>
        /// List all meeting's attendance
        /// </summary>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingAttendeeCollectionResult"/>.
        /// </returns>
        public MeetingAttendeeCollectionResult ReportMettingAttendance(string scoId, int startIndex = 0, int limit = 0, bool returnCurrentUsers = false)
        {
            // act: "report-meeting-attendance"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingAttendance, string.Format(CommandParams.ScoId, scoId).AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                       ? new MeetingAttendeeCollectionResult(status, MeetingAttendeeCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl, returnCurrentUsers))
                       : new MeetingAttendeeCollectionResult(status);
        }
        
        public QuizResponseCollectionResult ReportQuizInteractions(string scoId, int startIndex = 0, int limit = 0)
        {
            // act: "report-quiz-interactions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportQuizInteractions, string.Format(CommandParams.ScoId, scoId).AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                       ? new QuizResponseCollectionResult(status, QuizResponseCollectionParser.Parse(doc))
                       : new QuizResponseCollectionResult(status);
        }

        /// <summary>
        /// List all meeting's sessions
        /// </summary>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingSessionCollectionResult"/>.
        /// </returns>
        public MeetingSessionCollectionResult ReportMettingSessions(string scoId, int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingSessions, string.Format(CommandParams.ScoId, scoId).AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                       ? new MeetingSessionCollectionResult(status, MeetingSessionCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl))
                       : new MeetingSessionCollectionResult(status);
        }

        /// <summary>
        /// Provides information about each event the current user has attended or is scheduled to attend.
        /// The user can be either a host or a participant in the event. The events returned are those in the
        /// user’s my-events folder.
        /// To obtain information about all events on your Enterprise Server or in your Enterprise Hosted
        /// account, call SCO shortcuts to get the SCO id of the events folder. Then, call SCO contents
        /// with the SCO id to list all events.
        /// </summary>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// <see cref="EventInfo">EventInfo array</see>
        /// </returns>
        public EventCollectionResult ReportMyEvents(int startIndex = 0, int limit = 0)
        {
            // act: "report-my-events"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMyEvents, string.Empty.AppendPagingIfNeeded(startIndex, limit).TrimStart('&'), out status);

            return ResponseIsOk(doc, status)
                ? new EventCollectionResult(status, EventInfoCollectionParser.Parse(doc))
                : new EventCollectionResult(status);
        }

        /// <summary>
        /// Provides information about each event the current user has attended or is scheduled to attend.
        /// The user can be either a host or a participant in the event. The events returned are those in the
        /// user’s my-events folder.
        /// To obtain information about all events on your Enterprise Server or in your Enterprise Hosted
        /// account, call SCO shortcuts to get the SCO id of the events folder. Then, call SCO contents
        /// with the SCO id to list all events.
        /// </summary>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// <see cref="EventInfo">EventInfo array</see>
        /// </returns>
        public MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0)
        {
            // act: "report-my-meetings"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMyMeetings, string.Empty.AppendPagingIfNeeded(startIndex, limit).TrimStart('&'), out status);

            return ResponseIsOk(doc, status)
                ? new MeetingItemCollectionResult(status, MeetingItemCollectionParser.Parse(doc, string.Empty, "//my-meetings/meeting"))
                : new MeetingItemCollectionResult(status);
        }

        public MeetingItemCollectionResult ReportMyMeetings(MeetingPermissionId permission, int startIndex = 0, int limit = 0)
        {
            string filter = string.Empty;
            switch (permission)
            {
                case MeetingPermissionId.host:
                    filter = CommandParams.Permissions.Filter.PermissionId.Host;
                    break;
                case MeetingPermissionId.mini_host:
                    filter = CommandParams.Permissions.Filter.PermissionId.MiniHost;
                    break;
                case MeetingPermissionId.view:
                    filter = CommandParams.Permissions.Filter.PermissionId.View;
                    break;
            }
            // act: "report-my-meetings"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMyMeetings, filter.AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                ? new MeetingItemCollectionResult(status, MeetingItemCollectionParser.Parse(doc, string.Empty, "//my-meetings/meeting"))
                : new MeetingItemCollectionResult(status);
        }

        /// <summary>
        /// The get all events.
        /// </summary>
        /// <returns>
        /// The <see cref="EventCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetAllEvents()
        {
            return this.GetContentsByType(CommandParams.ShortcutTypes.Events);
        }

        /// <summary>
        /// The get all meetings.
        /// </summary>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetAllMeetings()
        {
            return this.GetContentsByType(CommandParams.ShortcutTypes.Meetings);
        }
        
        /// <summary>
        /// Reports curriculum taker results.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <returns>
        /// The <see cref="CurriculumTakerCollectionResult"/>.
        /// </returns>
        public CurriculumTakerCollectionResult ReportCurriculumTaker(string scoId, string principalId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Curriculum.ReportCurriculumTaker, string.Format(CommandParams.ScoIdAndUserId, scoId, principalId), out status);

            return ResponseIsOk(scos, status)
                ? new CurriculumTakerCollectionResult(status, CurriculumTakerCollectionParser.Parse(scos), scoId)
                : new CurriculumTakerCollectionResult(status);
        }

        /// <summary>
        /// The learning path info.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="LearningPathCollectionResult"/>.
        /// </returns>
        public LearningPathCollectionResult LearningPathInfo(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Curriculum.LearningPathInfo, string.Format(CommandParams.CurriculumIdAndScoId, scoId, scoId), out status);

            return ResponseIsOk(scos, status)
                ? new LearningPathCollectionResult(status, LearningPathCollectionParser.Parse(scos), scoId)
                : new LearningPathCollectionResult(status);
        }

        /// <summary>
        /// The learning path info.
        /// </summary>
        /// <param name="u">
        /// The u.
        /// </param>
        /// <returns>
        /// The <see cref="LearningPathCollectionResult"/>.
        /// </returns>
        public GenericResult LearningPathUpdate(LearningPathItem u)
        {
            StatusInfo status;
            this.requestProcessor.Process(Commands.Curriculum.LearningPathUpdate, string.Format(CommandParams.LearningPathUpdate, u.CurriculumId, u.CurrentScoId, u.TargetScoId, u.PathType), out status);
            return new GenericResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] GetContent(string scoId, out string error, string format = "zip")
        {
            var res = this.GetScoInfo(scoId);
            if (res.Success && res.ScoInfo != null && !string.IsNullOrEmpty(res.ScoInfo.UrlPath))
            {
                return this.GetContentByUrlPath(res.ScoInfo.UrlPath, format, out error);
            }

            error = res.Status == null
                        ? "Result is null"
                        : (string.IsNullOrWhiteSpace(res.Status.InnerXml)
                               ? res.Status.UnderlyingExceptionInfo.ToString()
                               : res.Status.InnerXml);
            return null;
        }

        /// <summary>
        /// The get content by url path.
        /// </summary>
        /// <param name="urlPath">
        /// The url path.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] GetContentByUrlPath(string urlPath, string format, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadData(downloadName, format, out error);
        }

        public byte[] GetContentByUrlPath2(string urlPath, string fileName, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadData2(downloadName, fileName, out error);
        }

        public byte[] GetSourceContentByUrlPath(string urlPath, string fileName, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadSourceData(downloadName, fileName, out error);
        }

        /// <summary>
        /// Uploads contents by SCO id.
        /// </summary>
        /// <param name="uploadScoInfo">
        /// The upload SCO Info.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public StatusInfo UploadContent(UploadScoInfo uploadScoInfo)
        {
            StatusInfo status;
            this.requestProcessor.ProcessUpload(Commands.Sco.Upload, string.Format(CommandParams.ScoUpload, uploadScoInfo.scoId, uploadScoInfo.summary, uploadScoInfo.title), uploadScoInfo, out status);
            return status;
        }

        /// <summary>
        /// The get Curriculum contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public CurriculumContentCollectionResult GetCurriculumContentsByScoId(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Curriculum.Contents, string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(scos, status)
                ? new CurriculumContentCollectionResult(status, CurriculumContentCollectionParser.Parse(scos), scoId)
                : new CurriculumContentCollectionResult(status);
        }

        /// <summary>
        /// Gets the type of the contents by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Contents by Type.</returns>
        /// TODO: should it be refactored Enum?
        public ScoContentCollectionResult GetContentsByType(string type)
        {
            StatusInfo status;

            var eventShortcut = this.GetShortcutByType(type, out status);

            return eventShortcut == null ? new ScoContentCollectionResult(status) : this.GetContentsByScoId(eventShortcut.ScoId);
        }

        /// <summary>
        /// The get SCO permissions info.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetScoPublicAccessPermissions(string scoId)
        {
            return this.GetPermissionsInfo(scoId, CommandParams.PrincipalIdPublicAccess);
        }

        public PermissionCollectionResult GetScoPermissions(string scoId, string principalId)
        {
            return this.GetPermissionsInfo(scoId, principalId);
        }

        /// <summary>
        /// The get meeting hosts.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetMeetingHosts(string meetingId)
        {
            return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.Host);
        }

        /// <summary>
        /// The get meeting presenters.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetMeetingPresenters(string meetingId)
        {
            return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.MiniHost);
        }

        /// <summary>
        /// The get meeting participants.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetMeetingParticipants(string meetingId)
        {
            return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.View);
        }

        /// <summary>
        /// The get meeting participants.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="PermissionCollectionResult"/>.
        /// </returns>
        public PermissionCollectionResult GetAllMeetingEnrollments(string meetingId)
        {
            return this.GetPermissionsInfo(meetingId, null, CommandParams.Permissions.Filter.PermissionId.All);
        }

        public PermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting SCO can't be empty", "meetingId");
            if (principalIds == null)
                throw new ArgumentNullException("principalIds");

            var filter = new StringBuilder(23 * principalIds.Count());
            foreach (string principalId in principalIds)
                filter.AppendFormat("&" + CommandParams.PrincipalByPrincipalId, UrlEncode(principalId));

            return this.GetPermissionsInfo(meetingId, null, filter.ToString());
        }

        /// <summary>
        /// Gets Group by name.
        /// </summary>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <param name="typeName">
        /// The type Name.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public StatusInfo AddToGroupByType(string principalId, string typeName)
        {
            var groups = this.GetGroupsByType(typeName);
            var group = groups.Item2.FirstOrDefault();
            return group != null ? this.AddToGroup(principalId, group.PrincipalId) : groups.Item1;
        }

        public StatusInfo AddToGroupByType(IEnumerable<string> principalIds, string typeName)
        {
            var groups = this.GetGroupsByType(typeName);
            var group = groups.Item2.FirstOrDefault();
            return group != null ? this.AddToGroup(principalIds, group.PrincipalId) : groups.Item1;
        }

        /// <summary>
        /// Gets Group by name.
        /// </summary>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <param name="typeName">
        /// The type Name.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public bool RemoveFromGroupByType(string principalId, string typeName)
        {
            var group = this.GetGroupsByType(typeName).Item2.FirstOrDefault();
            if (group != null)
            {
                return ResponseIsOk(this.RemoveFromGroup(principalId, group.PrincipalId));
            }

            return false;
        }

        /// <summary>
        /// The add to group.
        /// </summary>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo AddToGroup(string principalId, string groupId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Format(CommandParams.GroupMembership, groupId, principalId, "true"), out status);

            return status;
        }

        public StatusInfo AddToGroup(IEnumerable<string> principalIds, string groupId)
        {
            StatusInfo status;

            var trios = new List<string>(principalIds.Count());
            var paramBuilder = new StringBuilder();
            foreach (string principalId in principalIds)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.GroupMembership, groupId, principalId, "true");
                trios.Add(paramBuilder.ToString());
            }

            this.requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Join("&", trios), out status);
            return status;
        }

        /// <summary>
        /// The remove from group.
        /// </summary>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo RemoveFromGroup(string principalId, string groupId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Format(CommandParams.GroupMembership, groupId, principalId, "false"), out status);

            return status;
        }

        /// <summary>
        /// Gets Group by name.
        /// </summary>
        /// <param name="groupName">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalResult GetGroupByName(string groupName)
        {
            var groupsResult = this.GetGroupsByType("group");
            var status = groupsResult.Item1;
            var groups = groupsResult.Item2;
            var group = groups.FirstOrDefault(
                g => g.Name.Equals(groupName, StringComparison.InvariantCultureIgnoreCase));
            if (null != group)
            {
                return new PrincipalResult(status, group);
            }

            return new PrincipalResult(status);
        }

        /// <summary>
        /// Gets Group by name.
        /// </summary>
        /// <param name="type">
        /// The group type.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public Tuple<StatusInfo, IEnumerable<Principal>> GetGroupsByType(string type)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, "&filter-type=" + type, out status);
            if (ResponseIsOk(principals, status))
            {
                return new Tuple<StatusInfo, IEnumerable<Principal>>(status, PrincipalCollectionParser.Parse(principals));
            }

            return new Tuple<StatusInfo, IEnumerable<Principal>>(status, new List<Principal>());
        }

        public Tuple<StatusInfo, IEnumerable<Principal>> GetPrimaryGroupsByType(string type)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, "&filter-is-primary=true&filter-type=" + type, out status);
            if (ResponseIsOk(principals, status))
            {
                return new Tuple<StatusInfo, IEnumerable<Principal>>(status, PrincipalCollectionParser.Parse(principals));
            }

            return new Tuple<StatusInfo, IEnumerable<Principal>>(status, new List<Principal>());
        }
        
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

        /// <summary>
        /// The get curriculum by folder SCO id.
        /// </summary>
        /// <param name="folderScoId">
        /// The folder SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetCurriculumsByFolder(string folderScoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.FolderCurriculums, folderScoId), out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), folderScoId)
                : new ScoContentCollectionResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetMeetingRecordings(string scoId, bool includeMP4recordings = false)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(includeMP4recordings ? CommandParams.MeetingArchivesWithMP4 : CommandParams.MeetingArchives, scoId), out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), scoId)
                : new ScoContentCollectionResult(status);
        }

        /// <summary>
        /// The get recordings by SCO ids.
        /// </summary>
        /// <param name="scoIds">meeting/folder ids</param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>. Values item's FolderId is parent folder id or meeting id.
        /// </returns>
        public ScoContentCollectionResult GetMeetingRecordings(IEnumerable<string> scoIds, bool includeMP4recordings = false)
        {
            var result = new ScoContentCollectionResult(
                new StatusInfo { Code = StatusCodes.ok }, new List<ScoContent>());
            foreach (var id in scoIds)
            {
                var recordings = this.GetMeetingRecordings(id, includeMP4recordings);
                if (recordings.Success)
                {
                    result.Values = result.Values.Concat(recordings.Values);
                }
            }

            return result;
        }

        public CommonInfoResult GetCommonInfo()
        {
            //act: "common-info"
            const string commonInfoPath = "//results/common";
            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.CommonInfo, string.Empty, out status);

            return ResponseIsOk(doc, status)
               ? new CommonInfoResult(status, CommonInfoParser.Parse(doc.SelectSingleNode(commonInfoPath)))
               : new CommonInfoResult(status);

        }

        #endregion

        #region Write

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalSetup">The principal setup.</param>
        /// <returns>Status Info.</returns>
        public PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation = false)
        {
            // action=principal-update
            var commandParams = QueryStringBuilder.EntityToQueryString(principalSetup, isUpdateOperation);

            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Principal.Update, commandParams, out status);

            return new PrincipalResult(status, PrincipalParser.Parse(doc));
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <param name="newPassword">
        /// The new Password.
        /// </param>
        /// <returns>
        /// Status Info.
        /// </returns>
        public GenericResult PrincipalUpdatePassword(string principalId, string newPassword)
        {
            StatusInfo status;
            var parameters = string.Format(
                CommandParams.PrincipalUpdatePassword,
                principalId,
                UrlEncode(newPassword),
                UrlEncode(newPassword));
            this.requestProcessor.Process(Commands.Principal.UpdatePassword, parameters, out status);

            return new GenericResult(status);
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalDelete">The principal setup.</param>
        /// <returns>Status Info.</returns>
        public PrincipalResult PrincipalDelete(PrincipalDelete principalDelete)
        {
            // action=principal-update
            var commandParams = QueryStringBuilder.EntityToQueryString(principalDelete);

            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Principal.Delete, commandParams, out status);

            return new PrincipalResult(status, PrincipalParser.Parse(doc));
        }
        
        /// <summary>
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

        /// <summary>
        /// The update meeting feature for account
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="featureId">
        /// The feature Id.
        /// </param>
        /// <param name="enable">
        /// The enable.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo UpdateMeetingFeature(string accountId, MeetingFeatureId featureId, bool enable)
        {
            // act: "meeting-feature-update"
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.FeatureUpdate, string.Format(CommandParams.Features.Update, accountId, featureId.ToXmlString(), enable ? "true" : "false"), out status);

            return status;
        }

        /// <summary>
        /// The get ACL field.
        /// </summary>
        /// <param name="aclId">
        /// The ACL id.
        /// </param>
        /// <param name="fieldId">
        /// The field id.
        /// </param>
        /// <returns>
        /// The <see cref="FieldResult"/>.
        /// </returns>
        public FieldResult GetAclField(string aclId, AclFieldId fieldId)
        {
            StatusInfo status;

            var result = this.requestProcessor.Process(Commands.Sco.FieldInfo, string.Format(CommandParams.Features.FieldInfo, aclId, fieldId.ToXmlString()), out status);

            return ResponseIsOk(result, status)
                ? new FieldResult(status, FieldValueParser.Parse(result))
                : new FieldResult(status);
        }

        public FieldResult GetAclField(string aclId, string fieldId)
        {
            StatusInfo status;

            var result = this.requestProcessor.Process(Commands.Sco.FieldInfo, string.Format(CommandParams.Features.FieldInfo, aclId, fieldId), out status);

            return ResponseIsOk(result, status)
                ? new FieldResult(status, FieldValueParser.Parse(result))
                : new FieldResult(status);
        }

        public FieldCollectionResult GetAclFields(long aclId)
        {
            StatusInfo status;

            var result = this.requestProcessor.Process(Commands.Sco.FieldInfo, string.Format(CommandParams.Features.FieldInfoAll, aclId.ToString()), out status);

            return ResponseIsOk(result, status)
                ? new FieldCollectionResult(status, FieldCollectionParser.Parse(result))
                : new FieldCollectionResult(status);
        }

        /// <summary>
        /// The update ACL field.
        /// </summary>
        /// <param name="aclId">
        /// The ACL Id.
        /// </param>
        /// <param name="fieldId">
        /// The feature Id.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo UpdateAclField(string aclId, AclFieldId fieldId, string value)
        {
            // act: "acl-field-update"
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.FieldUpdate, string.Format(CommandParams.Features.FieldUpdate, aclId, fieldId.ToXmlString(), value), out status);

            return status;
        }

        public StatusInfo UpdateAclField(IEnumerable<AclFieldUpdateTrio> values)
        {
            // act: "acl-field-update"
            StatusInfo status;

            var trios = new List<string>(values.Count());
            var paramBuilder = new StringBuilder();
            foreach (AclFieldUpdateTrio trio in values)
            {
                paramBuilder.Length = 0;
                // "acl-id={0}&field-id={1}&value={2}"
                paramBuilder.AppendFormat(CommandParams.Features.FieldUpdate, trio.AclId, trio.FieldId, trio.Value);
                trios.Add(paramBuilder.ToString());
            }

            this.requestProcessor.Process(Commands.Sco.FieldUpdate, string.Join("&", trios), out status);

            return status;
        }


        /// <summary>
        /// The update meeting permission for user or a group.
        /// </summary>
        /// <param name="scoId">
        /// The meeting id.
        /// </param>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo UpdateScoPermissionForPrincipal(string scoId, string principalId, MeetingPermissionId permissionId)
        {
            switch (permissionId)
            {
                case MeetingPermissionId.host:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.host);
                case MeetingPermissionId.mini_host:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.mini_host);
                case MeetingPermissionId.view:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.view);
                case MeetingPermissionId.remove:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.remove);
                case MeetingPermissionId.denied:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.denied);
                case MeetingPermissionId.manage:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.manage);
                case MeetingPermissionId.publish:
                    return this.UpdatePermissionsInternal(scoId, principalId, PermissionId.publish);
                default:
                    throw new NotImplementedException();
            }
        }

        public StatusInfo UpdateScoPermissionForPrincipal(IEnumerable<PermissionUpdateTrio> values)
        {
            return this.UpdatePermissionsInternal(values);
        }

        #endregion

        #region internal routines

        #region Private Helpers

        /// <summary>
        /// Creates status.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="subCode">The sub code.</param>
        /// <param name="exceptionInfo">The exception info.</param>
        /// <returns>Status Info.</returns>
        private static StatusInfo CreateStatusInfo(StatusCodes code, StatusSubCodes subCode, Exception exceptionInfo)
        {
            return new StatusInfo
            {
                Code = code,
                SubCode = subCode,
                UnderlyingExceptionInfo = exceptionInfo
            };
        }

        /// <summary>
        /// The response is ok.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ResponseIsOk(XmlNode xml, StatusInfo status)
        {
            return status.Code == StatusCodes.ok && xml != null && xml.HasChildNodes;
        }

        /// <summary>
        /// The response is ok.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ResponseIsOk(StatusInfo status)
        {
            return status.Code == StatusCodes.ok;
        }

        #endregion

        /// <summary>
        /// Updates the permissions a principal has to access a SCO, using a trio of principal-id, ACL id,
        /// and permission-id. To update permissions for multiple principals or objects, specify
        /// multiple trios. You can update more than 200 permissions in a single call to permissions update.
        /// </summary>
        /// <param name="aclId">ACL id - required.</param>
        /// <param name="principalId">Principal id - required.</param>
        /// <param name="permissionId">Permission id - required.</param>
        /// <returns>Status Info.</returns>
        private StatusInfo UpdatePermissionsInternal(string aclId, string principalId, PermissionId permissionId)
        {
            // act: "permissions-update"
            StatusInfo status;

            this.requestProcessor.Process(Commands.Permissions.Update, string.Format(CommandParams.Permissions.Update, aclId, principalId, permissionId.ToXmlString()), out status);

            return status;
        }

        private StatusInfo UpdatePermissionsInternal(IEnumerable<PermissionUpdateTrio> values)
        {
            // act: "permissions-update"
            StatusInfo status;

            var trios = new List<string>(values.Count());
            var paramBuilder = new StringBuilder();
            foreach (PermissionUpdateTrio trio in values)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.Permissions.Update, trio.ScoId, trio.PrincipalId, trio.Permission.ToXmlString());
                trios.Add(paramBuilder.ToString());
            }

            this.requestProcessor.Process(Commands.Permissions.Update, string.Join("&", trios), out status);

            return status;
        }
        
        /// <summary>
        /// Returns the list of principals (users or groups) who have permissions to act on a SCO,
        /// principal, or account.
        /// </summary>
        /// <param name="aclId">*Required.
        /// The ID of a SCO, account, or principal
        /// that a principal has permission to act
        /// on. The ACL id is a SCO-id, principal id,
        /// or account-id in other calls.
        /// </param>
        /// <param name="principalId">Optional. 
        /// The ID of a user or group who has a
        /// permission (even if denied or not set) to
        /// act on a SCO, an account, or another principal.
        /// </param>
        /// <param name="filter">Optional filter.</param>
        /// <returns>Permissions result.</returns>
        private PermissionCollectionResult GetPermissionsInfo(string aclId, string principalId = null, string filter = null)
        {
            // action=permissions-info
            StatusInfo status;

            var commandParams = string.Format(CommandParams.Permissions.AclId, aclId);

            if (!string.IsNullOrWhiteSpace(principalId))
            {
                commandParams += "&" + string.Format(CommandParams.Permissions.PrincipalId, principalId);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                commandParams += string.Format("&{0}", filter);
            }

            var doc = this.requestProcessor.Process(Commands.Permissions.Info, commandParams, out status);

            return ResponseIsOk(doc, status)
                ? new PermissionCollectionResult(status, PermissionInfoCollectionParser.Parse(doc))
                : new PermissionCollectionResult(status);
        }

        /// <summary>
        /// Performs login procedure.
        /// </summary>
        /// <param name="login">Valid Adobe Connect account name.</param>
        /// <param name="password">Valid Adobe Connect account password.</param>
        /// <param name="status">After successful login, <see cref="StatusInfo">status</see> contains session ID to be used for single-sign-on.</param>
        /// <returns><see cref="bool"/>Success or not.</returns>
        private bool LoginInternal(string login, string password, out StatusInfo status)
        {
            // action=login&login=bobs@acme.com&password=football&session=
            // cookie: BREEZESESSION
            status = new StatusInfo();

            try
            {
                var doc = this.requestProcessor.Process(
                    Commands.Login,
                    string.Format(
                        CommandParams.LoginParams,
                        UrlEncode(login),
                        UrlEncode(password)),
                    out status);

                return ResponseIsOk(doc, status);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return false;
        }
        
        #endregion
    }
}
