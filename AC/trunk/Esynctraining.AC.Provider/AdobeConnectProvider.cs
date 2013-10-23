namespace Esynctraining.AC.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
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
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class AdobeConnectProvider
    {
        #region Private Constants

        /// <summary>
        /// The SCO home path.
        /// </summary>
        private const string ScoHome = "//sco";

        #endregion

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
        public AdobeConnectProvider()
            : this(new ConnectionDetails
            {
                ServiceUrl = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringServiceUrl],
                EventMaxParticipants = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringEventMaxParticipants].ParseIntWithDefault(AdobeConnectProviderConstants.DefaultEventMaxParticipants),

                Proxy = new ProxyCredentials
                {
                    Url = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringProxyUrl],
                    Domain = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringProxyDomain],
                    Login = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringProxyLogin],
                    Password = ConfigurationManager.AppSettings[AdobeConnectProviderConstants.ConfigStringProxyPassword]
                }
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdobeConnectProvider"/> class.
        /// </summary>
        /// <param name="connectionDetails">
        /// The connection details.
        /// </param>
        /// <exception cref="ArgumentException">
        /// ConnectionDetails should not be null
        /// </exception>
        public AdobeConnectProvider(ConnectionDetails connectionDetails)
        {
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
            
            return new LoginResult(statusInfo, success ? this.GetUserInfo() : null);
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

            var user = this.GetUserInfo(out status);

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

        /// <summary>
        /// Gets user info.
        /// </summary>
        /// <returns>
        /// The <see cref="UserInfo"/>.
        /// </returns>
        public UserInfo GetUserInfo()
        {
            StatusInfo status;

            return this.GetUserInfo(out status);
        }

        /// <summary>
        /// Returns information about currently logged in user
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        ///   <see cref="UserInfo"/>
        /// </returns>
        public UserInfo GetUserInfo(out StatusInfo status)
        {
            // action=common-info
            var doc = this.requestProcessor.Process(Commands.CommonInfo, null, out status);

            if (!ResponseIsOk(doc, status))
            {
                return null;
            }

            try
            {
                return UserInfoParser.Parse(doc);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }

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

        /// <summary>
        /// List all meeting's attendance
        /// </summary>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingAttendeeCollectionResult"/>.
        /// </returns>
        public MeetingAttendeeCollectionResult ReportMettingAttendance(string scoId)
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingAttendance, string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(doc, status)
                       ? new MeetingAttendeeCollectionResult(status, MeetingAttendeeCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl))
                       : new MeetingAttendeeCollectionResult(status);
        }

        /// <summary>
        /// The search SCO by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult SearchScoByName(string name)
        {
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Sco.SearchByField, string.Format(CommandParams.FieldAndQuery, "name", name), out status);

            return ResponseIsOk(doc, status)
                       ? new ScoContentCollectionResult(status, ScoSearchByFieldParser.Parse(doc))
                       : new ScoContentCollectionResult(status);
            
        }

        /// <summary>
        /// List all meeting's quiz
        /// </summary>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingAttendeeCollectionResult"/>.
        /// </returns>
        public QuizResponseCollectionResult ReportQuizInteractions(string scoId)
        {
            // act: "report-quiz-interactions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportQuizInteractions, string.Format(CommandParams.ScoId, scoId), out status);

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
        /// <returns>
        /// The <see cref="MeetingSessionCollectionResult"/>.
        /// </returns>
        public MeetingSessionCollectionResult ReportMettingSessions(string scoId)
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingSessions, string.Format(CommandParams.ScoId, scoId), out status);

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
        /// <returns><see cref="EventInfo">EventInfo array</see></returns>
        public EventCollectionResult ReportMyEvents()
        {
            // act: "report-my-events"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMyEvents, null, out status);

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
        /// <returns><see cref="EventInfo">EventInfo array</see></returns>
        public MeetingItemCollectionResult ReportMyMeetings()
        {
            // act: "report-my-meetings"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMyMeetings, null, out status);

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
        /// The get SCO info.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult GetScoInfo(string scoId)
        {
            // act: "sco-info"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Sco.Info, string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(doc, status)
                ? new ScoInfoResult(status, ScoInfoParser.Parse(doc.SelectSingleNode(ScoHome)))
                : new ScoInfoResult(status);
        }

        /// <summary>
        /// The get SCO info.
        /// </summary>
        /// <param name="scoUrl">
        /// The SCO url.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult GetScoByUrl(string scoUrl)
        {
            // act: "sco-by-url"
            StatusInfo status;

            var pathId = scoUrl.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var urlPath = string.Format("/{0}/", pathId);
            var doc = this.requestProcessor.Process(Commands.Sco.ByUrl, string.Format(CommandParams.UrlPath, urlPath), out status);

            return ResponseIsOk(doc, status)
                ? new ScoInfoResult(status, ScoInfoParser.Parse(doc.SelectSingleNode(ScoHome)))
                : new ScoInfoResult(status);
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
        public ScoContentCollectionResult GetContentsByScoId(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), scoId)
                : new ScoContentCollectionResult(status);
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
        /// Provides a complete list of users and groups, including primary groups.
        /// </summary>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllPrincipals()
        {
            return this.GetGroupPrincipalUsers(null);
        }

        /// <summary>
        /// Provides a list of users by email
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllByEmail(string email)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Format(CommandParams.PrincipalByEmail, email), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        /// <summary>
        /// Gets all principals if no Group Id specified.
        /// Otherwise gets only users of the specified Group.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.IsNullOrWhiteSpace(groupId) ? null : string.Format(CommandParams.PrincipalGroupIdUsersOnly, groupId), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        /// <summary>
        /// Gets all principals if no Group Id specified.
        /// Otherwise gets only users of the specified Group.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupUsers(string groupId)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Format("&group-id={0}&filter-is-member=true", groupId), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
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
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, "&filter-type=group", out status);
            if (ResponseIsOk(principals, status))
            {
                var groups = PrincipalCollectionParser.Parse(principals);
                var group = groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.InvariantCultureIgnoreCase));
                if (null != group)
                {
                    return new PrincipalResult(status, group);
                }
            }

            return new PrincipalResult(status);
        }

        /// <summary>
        /// The report meeting transactions.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="TransactionCollectionResult"/>.
        /// </returns>
        public TransactionCollectionResult ReportMeetingTransactions(string meetingId)
        {
            // act: "report-bulk-consolidated-transactions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkConsolidatedTransactions, string.Format(CommandParams.ReportBulkConsolidatedTransactionsFilters.MeetingScoId, meetingId), out status);

            return ResponseIsOk(doc, status)
                ? new TransactionCollectionResult(status, TransactionInfoCollectionParser.Parse(doc))
                : new TransactionCollectionResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult ReportRecordings()
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkObjects, CommandParams.ReportBulkObjectsFilters.Recording, out status);

            if (ResponseIsOk(doc, status))
            {
                var result = new ScoContentCollectionResult(
                    status, ScoRecordingCollectionParser.Parse(doc));

                return result;
            }

            return new ScoContentCollectionResult(status);
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
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        public ScoContentResult GetScoContent(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Info, string.Format(CommandParams.ScoId, scoId), out status);

            // ReSharper disable once InconsistentNaming
            const string scoPath = "//results/sco";

            return ResponseIsOk(scos, status)
                       // ReSharper disable once AssignNullToNotNullAttribute
                ? new ScoContentResult(status, ScoContentParser.Parse(scos.SelectNodes(scoPath).Cast<XmlNode>().FirstOrDefault()))
                : new ScoContentResult(status);
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
        public ScoContentCollectionResult GetMeetingRecordings(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.MeetingArchives, scoId), out status);

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
        public ScoContentCollectionResult GetMeetingRecordings(IEnumerable<string> scoIds)
        {
            var result = new ScoContentCollectionResult(
                new StatusInfo { Code = StatusCodes.ok }, new List<ScoContent>());
            foreach (var id in scoIds)
            {
                var recordings = this.GetMeetingRecordings(id);
                if (recordings.Success)
                {
                    result.Values = result.Values.Concat(recordings.Values);
                }
            }

            return result;
        }

        #endregion

        #region Write

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalSetup">The principal setup.</param>
        /// <returns>Status Info.</returns>
        public PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup)
        {
            // action=principal-update
            var commandParams = QueryStringBuilder.EntityToQueryString(principalSetup);

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

            this.requestProcessor.Process(Commands.Principal.UpdatePassword, string.Format(CommandParams.PrincipalUpdatePassword, principalId, newPassword, newPassword), out status);

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
        /// The create SCO.
        /// </summary>
        /// <typeparam name="T">
        /// SCO update item.
        /// </typeparam>
        /// <param name="scoUpdateItem">
        /// The SCO update item.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult CreateSco<T>(T scoUpdateItem)
            where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
            {
                return new ScoInfoResult(new StatusInfo { Code = StatusCodes.internal_error });
            }

            if (string.IsNullOrEmpty(scoUpdateItem.FolderId))
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "FolderId must be set to create new item")));
            }

            if (scoUpdateItem.Type == ScoType.not_set)
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "ScoType must be set")));
            }

            scoUpdateItem.ScoId = null;

            return this.ScoUpdate(scoUpdateItem, false);
        }

        /// <summary>
        /// The update SCO.
        /// </summary>
        /// <typeparam name="T">
        /// SCO update item.
        /// </typeparam>
        /// <param name="scoUpdateItem">
        /// The SCO update item.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult UpdateSco<T>(T scoUpdateItem)
            where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
            {
                return new ScoInfoResult(new StatusInfo { Code = StatusCodes.internal_error });
            }

            if (string.IsNullOrEmpty(scoUpdateItem.ScoId))
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "scoId must be set to update existing item")));
            }

            scoUpdateItem.FolderId = null;

            return this.ScoUpdate(scoUpdateItem, true);
        }

        /// <summary>
        /// Deletes one or more objects (SCOs).
        /// If the SCO-id you specify is for a folder, all the contents of the specified folder are deleted. To
        /// delete multiple SCOs, specify multiple SCO-id parameters.
        /// You can use a call such as SCO-contents to check the ref-count of the SCO, which is the
        /// number of other SCOs that reference this SCO. If the SCO has no references, you can safely
        /// remove it, and the server reclaims the space.
        /// If the SCO has references, removing it can cause the SCOs that reference it to stop working,
        /// or the server not to reclaim the space, or both. For example, if a course references a quiz
        /// presentation, removing the presentation might make the course stop working.
        /// As another example, if a meeting has used a content SCO (such as a presentation or video),
        /// there is a reference from the meeting to the SCO. Deleting the content SCO does not free
        /// disk space, because the meeting still references it.
        /// To delete a SCO, you need at least manage permission (see permission-id for details). Users
        /// who belong to the built-in authors group have manage permission on their own content
        /// folder, so they can delete content within it.
        /// </summary>
        /// <param name="scoId">The SCO id.</param>
        /// <returns>Status Info.</returns>
        public StatusInfo DeleteSco(string scoId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.Delete, string.Format(CommandParams.ScoId, scoId), out status);

            return status;
        }

        /// <summary>
        /// The move SCO.
        /// </summary>
        /// <param name="folderId">
        /// The folder id.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo MoveSco(string folderId, string scoId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.Move, string.Format(CommandParams.Move, folderId, scoId), out status);
            return status;
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

        /// <summary>
        /// The get shortcut by type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="ScoShortcut"/>.
        /// </returns>
        private ScoShortcut GetShortcutByType(string type, out StatusInfo status)
        {
            var shortcuts = this.requestProcessor.Process(Commands.Sco.Shortcuts, null, out status);

            return !ResponseIsOk(shortcuts, status) ? null : ShortcutCollectionParser.GetByType(shortcuts, type);
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
                var doc = this.requestProcessor.Process(Commands.Login, string.Format(CommandParams.LoginParams, login, password), out status);

                return ResponseIsOk(doc, status);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return false;
        }

        /// <summary>
        /// Creates metadata for a SCO, or updates existing metadata describing a SCO.
        /// Call SCO-update to create metadata only for SCOs that represent content, including
        /// meetings. You also need to upload content files with either SCO-upload or Connect Enterprise Manager.
        /// You must provide a folder-id or a SCO id, but not both. If you pass a folder-id, SCO-update
        /// creates a new SCO and returns a SCO id. If the SCO already exists and you pass a
        /// SCO-id, SCO-update updates the metadata describing the SCO.
        /// After you create a new SCO with SCO-update, call permissions-update to specify which
        /// users and groups can access it.
        /// </summary>
        /// <typeparam name="T">
        /// Base update item.
        /// </typeparam>
        /// <param name="meetingUpdateItem">
        /// The meeting item.
        /// </param>
        /// <param name="isUpdate">
        /// Is Update.
        /// </param>
        /// <returns>
        /// Save Meeting Result.
        /// </returns>
        private ScoInfoResult ScoUpdate<T>(T meetingUpdateItem, bool isUpdate)
            where T : ScoUpdateItemBase
        {
            if (meetingUpdateItem == null)
            {
                return null;
            }

            var commandParams = QueryStringBuilder.EntityToQueryString(meetingUpdateItem);

            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.Sco.Update, commandParams, out status);

            if (!ResponseIsOk(doc, status))
            {
                return new ScoInfoResult(status);
            }

            if (isUpdate)
            {
                return this.GetScoInfo(meetingUpdateItem.ScoId);
            }

            // notice: no '/sco' will be returned during update
            var detailNode = doc.SelectSingleNode(ScoHome);

            if (detailNode == null || detailNode.Attributes == null)
            {
                return new ScoInfoResult(status);
            }

            ScoInfo meetingDetail = null;

            try
            {
                meetingDetail = ScoInfoParser.Parse(detailNode);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
                status.Code = StatusCodes.invalid;
                status.SubCode = StatusSubCodes.format;
                status.UnderlyingExceptionInfo = ex;

                // delete meeting
                // [DD]: why would you do that?!..
                // if (meetingDetail != null && !string.IsNullOrEmpty(meetingDetail.scoId))
                // {
                // this.DeleteSco(meetingDetail.scoId);
                // }
            }

            return new ScoInfoResult(status, meetingDetail);
        }

        #endregion
    }
}
