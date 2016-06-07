using System;
using System.Collections.Generic;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectProxy
    {
        string ApiUrl { get; }

        StatusInfo AddToGroup(IEnumerable<string> principalIds, string groupId);
        //StatusInfo AddToGroup(string principalId, string groupId);
        StatusInfo AddToGroupByType(IEnumerable<string> principalIds, string typeName);
        StatusInfo AddToGroupByType(string principalId, string typeName);
        CancelRecordingJobResult CancelRecordingJob(string jobRecordingScoId);
        ScoInfoResult CreateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase;
        StatusInfo DeleteSco(string scoId);
        //FieldResult GetAclField(string aclId, AclFieldId fieldId);
        FieldCollectionResult GetAclFields(long aclId);
        PrincipalCollectionResult GetAllByEmail(string email);
        PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails);
        PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm);
        PrincipalCollectionResult GetAllByLogin(string login);
        PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins);
        PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind);
        //ScoContentCollectionResult GetAllEvents();
        PermissionCollectionResult GetAllMeetingEnrollments(string meetingId);

        PermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds, out bool meetingExistsInAC);
        //ScoContentCollectionResult GetAllMeetings();
        PrincipalCollectionResult GetAllPrincipal();
        PrincipalCollectionResult GetAllPrincipals();
        CommonInfoResult GetCommonInfo();
        //byte[] GetContent(string scoId, out string error, string format = "zip");
        byte[] GetContentByUrlPath(string urlPath, string format, out string error);
        ScoContentCollectionResult GetContentsByScoId(string scoId);
        ScoContentCollectionResult GetContentsByType(string type);
        //GeneratedRecordingJobCollectionResult GetConvertedRecordingsList(string recordingScoId);
        //CurriculumContentCollectionResult GetCurriculumContentsByScoId(string scoId);
        //ScoContentCollectionResult GetCurriculumsByFolder(string folderScoId);
        //PrincipalResult GetGroupByName(string groupName);
        // PrincipalCollectionResult GetGroupPrincipalUsers(string groupId);
        PrincipalCollectionResult GetGroupPrincipalUsers(string groupId, string principalId);
        Tuple<StatusInfo, IEnumerable<Principal>> GetGroupsByType(string type);
        Tuple<StatusInfo, IEnumerable<Principal>> GetPrimaryGroupsByType(string type);
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

        RecordingCollectionResult GetRecordingsList(string folderId, int skip, int take, string propertySortBy, SortOrder order, bool excludeMp4 = false);

        ScoInfoResult GetScoByUrl(string scoUrl);
        ScoContentResult GetScoContent(string scoId);

        ScoContentCollectionResult GetScoExpandedContent(string scoId);

        ScoContentCollectionResult GetScoExpandedContentByName(string scoId, string name);

        ScoContentCollectionResult GetScoExpandedContentByNameLike(string scoId, string nameLikeCriteria);

        ScoInfoResult GetScoInfo(string scoId);
        PermissionCollectionResult GetScoPublicAccessPermissions(string scoId);
        PermissionCollectionResult GetScoPermissions(string scoId, string principalId);
        ScoShortcut GetShortcutByType(string type);
        //UserInfo GetUserInfo();
        //UserInfo GetUserInfo(out StatusInfo status);
        //LearningPathCollectionResult LearningPathInfo(string scoId);
        //GenericResult LearningPathUpdate(LearningPathItem u);
        LoginResult Login(UserCredentials credentials);
        //LoginResult LoginWithSessionId(string sessionId);
        //void Logout();
        //StatusInfo MoveSco(string folderId, string scoId);
        PrincipalResult PrincipalDelete(PrincipalDelete principalDelete);
        PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation);
        PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation, bool throwOnAdobeError);
        GenericResult PrincipalUpdatePassword(string principalId, string newPassword);
        StatusInfo RemoveFromGroup(string principalId, string groupId);
        bool RemoveFromGroupByType(string principalId, string typeName);
        //MeetingItemCollectionResult ReportAllMeetings();
        MeetingItemCollectionResult ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0);
        //CurriculumTakerCollectionResult ReportCurriculumTaker(string scoId, string principalId);
        //TransactionCollectionResult ReportMeetingTransactions(string meetingId, int startIndex = 0, int limit = 0);
        TransactionCollectionResult ReportMeetingTransactionsForPrincipal(string principalId, int startIndex = 0, int limit = 0);
        MeetingAttendeeCollectionResult ReportMettingAttendance(string scoId, int startIndex = 0, int limit = 0, bool returnCurrentUsers = false);
        MeetingSessionCollectionResult ReportMettingSessions(string scoId, int startIndex = 0, int limit = 0);
        //EventCollectionResult ReportMyEvents(int startIndex = 0, int limit = 0);
        MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0);
        //QuizResponseCollectionResult ReportQuizInteractions(string scoId, int startIndex = 0, int limit = 0);
        //ScoContentCollectionResult ReportRecordings(int startIndex = 0, int limit = 0);
        RecordingJobResult ScheduleRecordingJob(string recordingScoId);
        //ScoContentCollectionResult SearchScoByDescription(string description);
        ScoContentCollectionResult SearchScoByName(string name);

        TelephonyProviderCollectionResult TelephonyProviderList(string principalId);
        TelephonyProfilesCollectionResult TelephonyProfileList(string principalId);
        TelephonyProfileInfoResult TelephonyProfileInfo(string profileId);
        TelephonyProfileInfoResult TelephonyProfileUpdate(TelephonyProfileUpdateItem updateItem, bool isUpdate);


        StatusInfo UpdateAclField(string aclId, AclFieldId fieldId, string value);
        //StatusInfo UpdateMeetingFeature(string accountId, MeetingFeatureId featureId, bool enable);
        StatusInfo UpdatePublicAccessPermissions(string aclId, PermissionId permissionId);
        StatusInfo UpdatePublicAccessPermissions(string aclId, SpecialPermissionId permissionId);
        ScoInfoResult UpdateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase;
        StatusInfo UpdateScoPermissionForPrincipal(IEnumerable<PermissionUpdateTrio> values);
        StatusInfo UpdateScoPermissionForPrincipal(string scoId, string principalId, MeetingPermissionId permissionId);
        StatusInfo UploadContent(UploadScoInfo uploadScoInfo);

        UserCollectionResult ReportGuestsByEmail(string email);
        UserCollectionResult ReportGuestsByLogin(string login);

        ScoInfoResult SeminarSessionScoUpdate(SeminarSessionScoUpdateItem item);
        SeminarLicensesCollectionResult GetSeminarLicenses(string scoId, bool returnUserSeminars = false);
        RecordingCollectionResult GetSeminarSessionRecordingsList(string seminarId, string seminarSessionId);
    }
}
