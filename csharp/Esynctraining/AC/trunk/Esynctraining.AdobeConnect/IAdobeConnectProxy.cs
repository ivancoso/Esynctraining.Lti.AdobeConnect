using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectProxy
    {
        Uri AdobeConnectRoot { get; }

        StatusInfo AddToGroup(IEnumerable<string> principalIds, string groupId);
        //StatusInfo AddToGroup(string principalId, string groupId);
        StatusInfo AddToGroupByType(IEnumerable<string> principalIds, PrincipalType type);
        StatusInfo AddToGroupByType(string principalId, PrincipalType type);
        CancelRecordingJobResult CancelRecordingJob(string jobRecordingScoId);
        ScoInfoResult CreateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase;
        StatusInfo DeleteSco(string scoId);
        //FieldResult GetAclField(string aclId, AclFieldId fieldId);
        FieldCollectionResult GetAclFields(long aclId);
        PrincipalCollectionResult GetAllByEmail(string email);
        PrincipalCollectionResult GetAllByEmailAndType(string email, PrincipalType principalType);
        PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails);
        PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm);
        PrincipalCollectionResult GetAllByLogin(string login);
        PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins);
        PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind);

        PrincipalInfoResult GetPrincipalInfo(string principalId);

        //ScoContentCollectionResult GetAllEvents();

        MeetingPermissionCollectionResult GetAllMeetingEnrollments(string meetingId);

        MeetingPermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds, out bool meetingExistsInAC);
        PrincipalCollectionResult GetAllPrincipals();
        CommonInfoResult GetCommonInfo();

        Task<(MemoryStream ms, string error)> GetContentAsync(string scoId, string format = "zip");

        byte[] GetContent(string scoId, out string error, string format = "zip");

        byte[] GetSourceContent(string urlPath, out string error, string format = "zip");

        byte[] GetContentByUrlPath(string urlPath, string format, out string error);

        ScoContentCollectionResult GetContentsByScoId(string scoId);

        ScoContentCollectionResult GetContents(ScoContentsFilter filter);

        ScoContentCollectionResult GetContentsByScoIdSourceScoId(string scoId, string filterSourceScoId);

        ScoContentCollectionResult GetContentsByType(string type);
        //GeneratedRecordingJobCollectionResult GetConvertedRecordingsList(string recordingScoId);
        //CurriculumContentCollectionResult GetCurriculumContentsByScoId(string scoId);
        //ScoContentCollectionResult GetCurriculumsByFolder(string folderScoId);
        //PrincipalResult GetGroupByName(string groupName);
        // PrincipalCollectionResult GetGroupPrincipalUsers(string groupId);
        PrincipalCollectionResult GetGroupPrincipalUsers(string groupId, string principalId);
        PrincipalCollectionResult GetGroupsByType(PrincipalType type);
        PrincipalCollectionResult GetPrimaryGroupsByType(PrincipalType type);
        PrincipalCollectionResult GetGroupUsers(string groupId);
        //PermissionCollectionResult GetMeetingHosts(string meetingId);
        //PermissionCollectionResult GetMeetingParticipants(string meetingId);
        //PermissionCollectionResult GetMeetingPresenters(string meetingId);
        ScoContentCollectionResult GetMeetingRecordings(IEnumerable<string> scoIds, bool includeMP4recordings = false);
        //ScoContentCollectionResult GetMeetingRecordings(string scoId, bool includeMP4recordings = false);
        //ScoContentCollectionResult GetMeetingsByFolder(string folderScoId);
        PrincipalInfoResult GetOneByPrincipalId(string principalId);
        //RecordingJobResult GetRecordingJob(string jobId);
        //RecordingJobCollectionResult GetRecordingJobsList(string folderId);
        RecordingCollectionResult GetRecordingsList(string folderId);
        
        RecordingCollectionResult GetRecordingsList(string folderId,
            int startIndex, int limit,
            string propertySortBy, SortOrder order,
            bool excludeMp4 = false,
            string scoId = null);


        ScoInfoResult GetScoByUrl(string scoUrl);

        ScoInfoByUrlResult GetScoByUrl2(string scoUrl);

        ScoContentResult GetScoContent(string scoId);

        ScoContentCollectionResult GetScoExpandedContent(string scoId);

        ScoContentCollectionResult GetScoExpandedContent(string scoId, string filter, int start = 0, int rows = 0);

        ScoContentCollectionResult GetScoExpandedContent(string scoId, ScoType scoType);
        ScoContentCollectionResult GetScoExpandedContentByName(string scoId, string name);
        ScoContentCollectionResult GetScoExpandedContentByIcon(string scoId, string icon, int start = 0, int rows = 0);
        ScoContentCollectionResult GetScoExpandedContentByNameLike(string scoId, string nameLikeCriteria);
        ScoContentCollectionResult SearchMeetingsByNameLike(string scoId, string nameLikeCriteria);

        ScoInfoResult GetScoInfo(string scoId);

        CollectionResult<ScoNav> GetScoNavigation(string scoId);

        PermissionCollectionResult GetScoPublicAccessPermissions(string scoId, bool skipAcError = false);

        /// <summary>
        /// Returns permissions for SCO (SCOs other than meetings or courses, e.g. files\folders)
        /// Returns only records with view\publish\manage\denied permissions.
        /// </summary>
        PermissionCollectionResult GetScoPermissions(string scoId);

        PermissionCollectionResult GetScoPermissions(string scoId, string principalId);

        IEnumerable<ScoShortcut> GetShortcuts();

        ScoShortcut GetShortcutByType(string type);

        ScoShortcut GetShortcutByType(string type, out StatusInfo status);

        //UserInfo GetUserInfo();
        //UserInfo GetUserInfo(out StatusInfo status);
        //LearningPathCollectionResult LearningPathInfo(string scoId);
        //GenericResult LearningPathUpdate(LearningPathItem u);
        LoginResult Login(UserCredentials credentials);
        //LoginResult LoginWithSessionId(string sessionId);
        //void Logout();
        StatusInfo MoveSco(string folderId, string scoId);
        PrincipalResult PrincipalDelete(PrincipalDelete principalDelete);
        PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation);
        PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation, bool throwOnAdobeError);
        GenericResult PrincipalUpdatePassword(string principalId, string newPassword);
        GenericResult PrincipalUpdateType(string principalId, PrincipalType type);
        StatusInfo RemoveFromGroup(string principalId, string groupId);
        bool RemoveFromGroupByType(string principalId, PrincipalType type);

        #region Reports

        CollectionResult<ReportBulkObjectItem> ReportBulkObjects(IEnumerable<string> scoIds);

        /// <summary>
        /// Uses filter-type=xxx 
        /// </summary>
        CollectionResult<ReportBulkObjectItem> ReportBulkObjectsByType(IEnumerable<string> types, int startIndex = 0, int limit = 0);

        /// <summary>
        /// NOTE: can return too much data and take a lot of time to complete.
        /// Consider using filter.
        /// Sorts by sco-id.
        /// </summary>
        CollectionResult<ReportBulkObjectItem> ReportAllMeetings(string filter = null, int startIndex = 0, int limit = 0);

        CollectionResult<ReportBulkObjectItem> ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0);

        //CurriculumTakerCollectionResult ReportCurriculumTaker(string scoId, string principalId);
        //TransactionCollectionResult ReportMeetingTransactions(string meetingId, int startIndex = 0, int limit = 0);
        TransactionCollectionResult ReportBulkConsolidatedTransactions(string filter, int startIndex = 0, int limit = 0);

        TransactionCollectionResult ReportRecordingTransactions(IEnumerable<string> recordingScoIdList);

        TransactionCollectionResult ReportMeetingTransactionsForPrincipal(string principalId, int startIndex = 0, int limit = 0);

        CollectionResult<ReportActiveMeetingsItem> ReportActiveMeetings();

        MeetingAttendeeCollectionResult ReportMeetingAttendance(string scoId, int startIndex = 0, int limit = 0, bool returnCurrentUsers = false);

        EventParticipantsCompleteInformationCollectionResult ReportEventParticipantsCompleteInformation(string scoId);

        MeetingSessionCollectionResult ReportMeetingSessions(string scoId, string filter = null, int startIndex = 0, int limit = 0);

        //EventCollectionResult ReportMyEvents(int startIndex = 0, int limit = 0);
        EventRegistrationDetails GetEventRegistrationDetails(string scoId);

        EventInfoResult GetEventInfo(string scoId);

        EventCollectionResult GetEventList();

        CollectionResult<EventNotification> EventNotificationList(string eventScoId);

        MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0);

        MeetingItemCollectionResult ReportMyMeetings(MeetingPermissionId permission, int startIndex = 0, int limit = 0);

        CollectionResult<TrainingItem> ReportMyTraining(string filter = "", int startIndex = 0, int limit = 0);

        //QuizResponseCollectionResult ReportQuizInteractions(string scoId, int startIndex = 0, int limit = 0);
        //ScoContentCollectionResult ReportRecordings(int startIndex = 0, int limit = 0);

        //IEnumerable<ScoContentCollectionResult> ReportRecordingsPaged(int totalLimit = 0, string filter = null, string sort = null);

        ReportScoViewsContentCollectionResult ReportScoViews(string scoId);

        ReportUserTrainingsTakenCollectionResult ReportUserTrainingsTaken(string principalId);

        CollectionResult<AssetResponseInfo> ReportAssetResponseInfo(string meetingScoId, string interactionId);

        CollectionResult<QuizQuestionResponseItem> ReportQuizQuestionResponse(string meetingScoId);

        CollectionResult<QuizInteractionItem> ReportQuizInteractions(string meetingScoId);

        CollectionResult<QuizQuestionDistributionItem> ReportQuizQuestionDistribution(string meetingScoId);

        #endregion Reports

        RecordingJobResult ScheduleRecordingJob(string recordingScoId);
        //ScoContentCollectionResult SearchScoByDescription(string description);
        ScoContentCollectionResult SearchScoByName(string name);

        #region Telephony

        TelephonyProviderCollectionResult TelephonyProviderList(string principalId);
        TelephonyProfilesCollectionResult TelephonyProfileList(string principalId);
        TelephonyProfileInfoResult TelephonyProfileInfo(string profileId);
        TelephonyProfileInfoResult TelephonyProfileUpdate(TelephonyProfileUpdateItem updateItem, bool isUpdate);

        StatusInfo TelephonyProfileDelete(string profileId);

        #endregion Telephony

        StatusInfo UpdateAclFieldWithPasscode(string aclId, AclFieldId fieldId, string value, bool isPasscodeRequired);
        StatusInfo UpdateAclFieldWithPasscode(string aclId, string fieldId, string value, bool isPasscodeRequired);


        StatusInfo UpdateAclField(string aclId, AclFieldId fieldId, string value);
        StatusInfo UpdateAclField(string aclId, string fieldId, string value); //there could be custom Id for Acl field
        StatusInfo UpdateAclField(IEnumerable<AclFieldUpdateTrio> values);
        //StatusInfo UpdateMeetingFeature(string accountId, MeetingFeatureId featureId, bool enable);
        CollectionResult<RoomFeature> MeetingFeatureInfo(string accountId = null);
        StatusInfo UpdatePublicAccessPermissions(string aclId, PermissionId permissionId);
        StatusInfo UpdatePublicAccessPermissions(string aclId, SpecialPermissionId permissionId);
        ScoInfoResult UpdateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase;

        StatusInfo UpdateScoPermissions(IEnumerable<IPermissionUpdateTrio> values);

        StatusInfo UploadContent(UploadScoInfo uploadScoInfo);

        Task<StatusInfo> UploadContentAsync(UploadScoInfo uploadScoInfo);

        UserCollectionResult ReportGuestsByEmail(string email);
        UserCollectionResult ReportGuestsByLogin(string login);

        ScoInfoResult SeminarSessionScoUpdate(SeminarSessionScoUpdateItem item);
        CollectionResult<SharedSeminarLicenseSco> GetSharedSeminarLicenses(string scoId);
        CollectionResult<UserSeminarLicenseSco> GetUserSeminarLicenses(string scoId);

        RecordingCollectionResult GetSeminarSessionRecordingsList(string seminarId, string seminarSessionId);
        string GetAclField(string scoId, string aclId);
        CustomField GetCustomField(string name);
        CollectionResult<CustomField> GetAllCustomFields();
        CollectionResult<CustomField> GetCustomFieldsByObjectType(ObjectType type);
        CollectionResult<CustomField> GetCustomFields(string fieldName, ObjectType objectType);
        SingleObjectResult<CustomField> CustomFieldUpdate(CustomField value);

        EventLoginInfoResult EventLogin(string login, string password, string scoId);
        RegisterEventInfoResult RegisterToEvent(EventRegistrationFormFields eventDetails, bool sendPasswordFields = true);

        SaveEventResponse CreateEvent(SaveEventFields saveEventFields);
        SaveEventResponse EditEvent(SaveEventFields saveEventFields, string eventScoId, bool isTimezoneChanged);

        StatusInfo UpdateVirtualClassroomLicenseModel(string scoId, bool enableNamedVcLicenseModel);
    }
}
