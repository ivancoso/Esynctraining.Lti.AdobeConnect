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
        private readonly RequestProcessor requestProcessor;

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

            requestProcessor = new RequestProcessor(connectionDetails);
        }

        #endregion

        #region Login/Logout

        public LoginResult Login(UserCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            this.requestProcessor.SetSessionId(null);

            StatusInfo statusInfo;
            var success = LoginInternal(credentials.Login, credentials.Password, credentials.AccountId, out statusInfo);

            UserInfo user = null;
            if (success)
            {
                var commonInfo = GetCommonInfo();
                user = commonInfo.Success ? commonInfo.CommonInfo.User : null;
            }

            return new LoginResult(statusInfo, success ? user : null);
        }

        public LoginResult LoginWithSessionId(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Non-empty value expected", nameof(sessionId));

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

        public void Logout()
        {
            // action=logout
            StatusInfo status;

            this.requestProcessor.Process(Commands.Logout, null, out status);
        }

        #endregion

        #region Read

        /// <summary>
        /// List all meetings on the server. Internal paging logic to return ALL meetings without too-much-data error.
        /// </summary>
        /// <returns>
        /// <see cref="MeetingItem">Meeting list</see>
        /// *Note: all dates are GMT
        /// </returns>
        public MeetingItemCollectionResult ReportAllMeetings(string filter = null, int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-objects"
            // "&sort-sco-id=asc" - to have valid paging!
            return DoCallMeetingItemList(Commands.ReportBulkObjects, 
                CommandParams.ReportBulkObjectsFilters.Meeting
                .AppendFilter(filter)
                .AppendSortingIfNeeded("sco-id", SortOrder.Ascending), // TODO: better sorting??
                null, 0, int.MaxValue);
        }

        public MeetingItemCollectionResult ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0)
        {
            string filter = string.Format(CommandParams.ReportBulkObjectsFilters.ByNameLike, nameLikeCriteria);
            return ReportAllMeetings(filter, startIndex, limit);
        }
        
        public MeetingAttendeeCollectionResult ReportMeetingAttendance(string scoId, int startIndex = 0, int limit = 0, bool returnCurrentUsers = false)
        {
            // act: "report-meeting-attendance"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingAttendance, string.Format(CommandParams.ScoId, scoId).AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                ? new MeetingAttendeeCollectionResult(status, MeetingAttendeeCollectionParser.Parse(doc, returnCurrentUsers))
                : new MeetingAttendeeCollectionResult(status);
        }

        //public QuizResponseCollectionResult ReportQuizInteractions(string scoId, int startIndex = 0, int limit = 0)
        //{
        //    // act: "report-quiz-interactions"
        //    StatusInfo status;

        //    var doc = this.requestProcessor.Process(Commands.ReportQuizInteractions, string.Format(CommandParams.ScoId, scoId).AppendPagingIfNeeded(startIndex, limit), out status);

        //    return ResponseIsOk(doc, status)
        //               ? new QuizResponseCollectionResult(status, QuizResponseCollectionParser.Parse(doc))
        //               : new QuizResponseCollectionResult(status);
        //}

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
        public MeetingSessionCollectionResult ReportMeetingSessions(string meetingScoId, string filter = null, int startIndex = 0, int limit = 0)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));

            // act: "report-meeting-sessions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportMeetingSessions,
                string.Format(CommandParams.ScoId, meetingScoId)
                    .AppendFilter(filter)
                    .AppendPagingIfNeeded(startIndex, limit), 
                out status);

            return ResponseIsOk(doc, status)
                ? new MeetingSessionCollectionResult(status, MeetingSessionCollectionParser.Parse(doc))
                : new MeetingSessionCollectionResult(status);
        }
        
        //public EventCollectionResult ReportMyEvents(int startIndex = 0, int limit = 0)
        //{
        //    // act: "report-my-events"
        //    StatusInfo status;

        //    var doc = this.requestProcessor.Process(Commands.ReportMyEvents, string.Empty.AppendPagingIfNeeded(startIndex, limit).TrimStart('&'), out status);

        //    return ResponseIsOk(doc, status)
        //        ? new EventCollectionResult(status, EventInfoCollectionParser.Parse(doc))
        //        : new EventCollectionResult(status);
        //}

        public MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0)
        {
            return CallReportMyMeetings(string.Empty, startIndex, limit);
        }

        public MeetingItemCollectionResult ReportMyMeetings(MeetingPermissionId permission, int startIndex = 0, int limit = 0)
        {
            string filter = string.Format(CommandParams.Permissions.Filter.PermissionId.Format,
                permission.GetACEnum());
            return CallReportMyMeetings(filter, startIndex, limit);
        }


        private MeetingItemCollectionResult CallReportMyMeetings(string filter, int startIndex = 0, int limit = 0)
        {
            filter = filter
                .AppendSortingIfNeeded("date-begin", SortOrder.Descending) //default sorting in AC, otherwise paging might be incorrect 
                .AppendPagingIfNeeded(startIndex, limit);

            return DoCallMeetingItemList(Commands.ReportMyMeetings, filter, "//my-meetings/meeting", 0, int.MaxValue);
        }

        private MeetingItemCollectionResult DoCallMeetingItemList(string action, string filter, string xPath, int startIndex, int limit)
        {
            StatusInfo status;
            var doc = this.requestProcessor.Process(action, filter.AppendPagingIfNeeded(startIndex, limit), out status);
            var data = MeetingItemCollectionParser.Parse(doc, this.requestProcessor.AdobeConnectRoot, xPath);
            bool okResponse = ResponseIsOk(doc, status);

            if (!okResponse)
            {
                if (status.Code == StatusCodes.operation_size_error)
                {
                    int? actualAcLimit = status.TryGetSubCodeAsInt32();
                    if (actualAcLimit.HasValue)
                    {
                        return DoCallMeetingItemList(action, filter, xPath, startIndex, actualAcLimit.Value);
                    }
                }
                return new MeetingItemCollectionResult(status);
            }

            if (data.Count() < limit)
                return new MeetingItemCollectionResult(status, data);

            MeetingItemCollectionResult nextPage = DoCallMeetingItemList(action, filter, xPath, startIndex + limit, limit);
            if (!nextPage.Success)
                return nextPage;

            return new MeetingItemCollectionResult(status, data.Concat(nextPage.Values));
        }

        ///// <summary>
        ///// Reports curriculum taker results.
        ///// </summary>
        ///// <param name="scoId">
        ///// The SCO id.
        ///// </param>
        ///// <param name="principalId">
        ///// The principal Id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="CurriculumTakerCollectionResult"/>.
        ///// </returns>
        //public CurriculumTakerCollectionResult ReportCurriculumTaker(string scoId, string principalId)
        //{
        //    StatusInfo status;

        //    var scos = this.requestProcessor.Process(Commands.Curriculum.ReportCurriculumTaker, string.Format(CommandParams.ScoIdAndUserId, scoId, principalId), out status);

        //    return ResponseIsOk(scos, status)
        //        ? new CurriculumTakerCollectionResult(status, CurriculumTakerCollectionParser.Parse(scos), scoId)
        //        : new CurriculumTakerCollectionResult(status);
        //}

        ///// <summary>
        ///// The learning path info.
        ///// </summary>
        ///// <param name="scoId">
        ///// The SCO id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="LearningPathCollectionResult"/>.
        ///// </returns>
        //public LearningPathCollectionResult LearningPathInfo(string scoId)
        //{
        //    StatusInfo status;

        //    var scos = this.requestProcessor.Process(Commands.Curriculum.LearningPathInfo, string.Format(CommandParams.CurriculumIdAndScoId, scoId, scoId), out status);

        //    return ResponseIsOk(scos, status)
        //        ? new LearningPathCollectionResult(status, LearningPathCollectionParser.Parse(scos), scoId)
        //        : new LearningPathCollectionResult(status);
        //}

        ///// <summary>
        ///// The learning path info.
        ///// </summary>
        ///// <param name="u">
        ///// The u.
        ///// </param>
        ///// <returns>
        ///// The <see cref="LearningPathCollectionResult"/>.
        ///// </returns>
        //public GenericResult LearningPathUpdate(LearningPathItem u)
        //{
        //    StatusInfo status;
        //    this.requestProcessor.Process(Commands.Curriculum.LearningPathUpdate, string.Format(CommandParams.LearningPathUpdate, u.CurriculumId, u.CurrentScoId, u.TargetScoId, u.PathType), out status);
        //    return new GenericResult(status);
        //}

        ///// <summary>
        ///// The get Curriculum contents by SCO id.
        ///// </summary>
        ///// <param name="scoId">
        ///// The SCO id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="ScoContentCollectionResult"/>.
        ///// </returns>
        //public CurriculumContentCollectionResult GetCurriculumContentsByScoId(string scoId)
        //{
        //    StatusInfo status;

        //    var scos = this.requestProcessor.Process(Commands.Curriculum.Contents, string.Format(CommandParams.ScoId, scoId), out status);

        //    return ResponseIsOk(scos, status)
        //        ? new CurriculumContentCollectionResult(status, CurriculumContentCollectionParser.Parse(scos), scoId)
        //        : new CurriculumContentCollectionResult(status);
        //}

        public ScoContentCollectionResult GetContentsByType(string type)
        {
            StatusInfo status;

            var eventShortcut = this.GetShortcutByType(type, out status);

            return eventShortcut == null ? new ScoContentCollectionResult(status) : this.GetContentsByScoId(eventShortcut.ScoId);
        }

        ///// <summary>
        ///// The get curriculum by folder SCO id.
        ///// </summary>
        ///// <param name="folderScoId">
        ///// The folder SCO Id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="ScoContentCollectionResult"/>.
        ///// </returns>
        //public ScoContentCollectionResult GetCurriculumsByFolder(string folderScoId)
        //{
        //    StatusInfo status;

        //    var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.FolderCurriculums, folderScoId), out status);

        //    return ResponseIsOk(scos, status)
        //        ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), folderScoId)
        //        : new ScoContentCollectionResult(status);
        //}

        private ScoContentCollectionResult GetMeetingRecordings(string scoId, bool includeMP4recordings = false)
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
            IEnumerable<ScoContent> values = new List<ScoContent>();
            
            foreach (var id in scoIds)
            {
                var recordings = this.GetMeetingRecordings(id, includeMP4recordings);
                if (recordings.Success)
                {
                    values = values.Concat(recordings.Values);
                }
            }

            var result = new ScoContentCollectionResult(
                new StatusInfo { Code = StatusCodes.ok }, values);
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

        #endregion

        #region internal routines

        private CollectionResult<T> GetCollection<T>(string command, string args, string xmlRootName, string xmlElementName, Func<XmlNode, T> elementParser)
        {
            StatusInfo status;

            var doc = this.requestProcessor.Process(command, args, out status);

            return ResponseIsOk(doc, status)
                       ? new CollectionResult<T>(status, GenericCollectionParser<T>.Parse(
                           doc.SelectSingleNode(xmlRootName), xmlElementName, elementParser))
                       : new CollectionResult<T>(status);
        }

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

        private SingleObjectResult<T> GetResult<T>(string action, string parameters, string xPath, IEntityParser<T> parser) where T : class
        {
            StatusInfo status;
            var doc = this.requestProcessor.Process(action, parameters, out status);

            if (!ResponseIsOk(doc, status))
                return new SingleObjectResult<T>(status);

            var node = doc.SelectSingleNode(xPath);
            if (node == null)
                return new SingleObjectResult<T>(status);

            return new SingleObjectResult<T>(status, parser.Parse(node));
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

        private StatusInfo UpdatePermissionsInternal(IEnumerable<IPermissionUpdateTrio> values)
        {
            // act: "permissions-update"
            StatusInfo status;

            var trios = new List<string>(values.Count());
            var paramBuilder = new StringBuilder();
            foreach (IPermissionUpdateTrio trio in values.Where(x => x.Permission != PermissionId.none))
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
        private PermissionCollectionResult GetPermissionsInfo(string aclId, string filter)
        {
            if (string.IsNullOrWhiteSpace(aclId))
                throw new ArgumentException("Non-empty value expected", nameof(aclId));
            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Non-empty value expected", nameof(filter));

            // action=permissions-info
            StatusInfo status;

            var commandParams = string.Format(CommandParams.Permissions.AclId + "&{1}", aclId, filter);

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
        /// <param name="accountId"></param>
        /// <returns><see cref="bool"/>Success or not.</returns>
        private bool LoginInternal(string login, string password, string accountId, out StatusInfo status)
        {
            // action=login&login=bobs@acme.com&password=football&session=
            // cookie: BREEZESESSION
            status = new StatusInfo();

            try
            {
                string parameters;
                if (string.IsNullOrEmpty(accountId))
                {
                   parameters = string.Format(
                   CommandParams.LoginParams,
                   UrlEncode(login),
                   UrlEncode(password));
                }
                else
                {
                   parameters = string.Format(
                   CommandParams.LoginWithAccountParams,
                   UrlEncode(login),
                   UrlEncode(password),
                   UrlEncode(accountId));
                }

                var doc = this.requestProcessor.Process(
                    Commands.Login,
                    parameters,
                    out status);

                return ResponseIsOk(doc, status);
            }
            catch (Exception ex)
            {
                status.UnderlyingExceptionInfo = ex;
                TraceTool.TraceException(ex);
            }

            return false;
        }

        #endregion

    }

}
