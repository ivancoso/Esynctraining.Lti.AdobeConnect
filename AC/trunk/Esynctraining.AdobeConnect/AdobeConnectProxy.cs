using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public class AdobeConnectProxy : IAdobeConnectProxy
    {
        private readonly AdobeConnectProvider _provider;
        private readonly ILogger _logger;


        public string ApiUrl { get; private set; }


        public AdobeConnectProxy(AdobeConnectProvider provider, ILogger logger, string apiUrl) 
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (logger == null)
                throw new ArgumentNullException("logger");

            _provider = provider;
            _logger = logger;
            ApiUrl = apiUrl;
        }

        public StatusInfo AddToGroup(IEnumerable<string> principalIds, string groupId)
        {
            return Execute(() => { return _provider.AddToGroup(principalIds, groupId); },
                String.Join(",", principalIds), groupId);
        }

        public StatusInfo AddToGroupByType(IEnumerable<string> principalIds, string typeName)
        {
            if (principalIds == null)
                throw new ArgumentNullException("principalIds");

            if (!principalIds.Any())
                throw new ArgumentException("Non empty principal id list required", "principalIds");

            //return Execute(() => _provider.AddToGroupByType(principalIds, typeName),
            //    string.Join(";", principalIds), typeName);
            StatusInfo result;
            try
            {
                result = _provider.AddToGroupByType(principalIds, typeName);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "PrincipalIds:{0}.TypeName:{1}.", string.Join(";", principalIds), typeName);
                throw new AdobeConnectException("AddToGroupByType exception", ex);
            }

            if (result.Code != StatusCodes.ok)
            {
                //ACLTI-385
                // ACU: User cannot be added to Meeting Hosts folder when User Meetings folder exists
                if ((result.Code == StatusCodes.invalid)
                    && (result.SubCode == StatusSubCodes.duplicate)
                    && (result.InvalidField == "unknown")
                    && (result.Type == "unavailable"))
                    return result;

                if (result.SubCode == StatusSubCodes.no_quota
                    && result.Type == "num-of-members-quota")
                {
                    throw new WarningMessageException(Resources.Messages.AdobeConnectMeetingHostQuota);
                }

                string msg = string.Format("[AdobeConnectProxy Error] {0}. PrincipalIds:{1}.TypeName:{2}.",
                    result.GetErrorInfo(),
                    string.Join(";", principalIds),
                    typeName);
                _logger.Error(msg);

                throw new AdobeConnectException(result);
            }

            return result;
        }

        public StatusInfo AddToGroupByType(string principalId, string typeName)
        {
            //return Execute(() => _provider.AddToGroupByType(principalId, typeName),
            //    principalId, typeName);

            StatusInfo result;
            try
            {
                result = _provider.AddToGroupByType(principalId, typeName);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "PrincipalId:{0}.TypeName:{1}.", principalId, typeName);
                throw new AdobeConnectException("AddToGroupByType exception", ex);
            }

            if (result.Code != StatusCodes.ok)
            {
                //ACLTI-385
                // ACU: User cannot be added to Meeting Hosts folder when User Meetings folder exists
                if ((result.Code == StatusCodes.invalid)
                    && (result.SubCode == StatusSubCodes.duplicate)
                    && (result.InvalidField == "unknown")
                    && (result.Type == "unavailable"))
                    return result;

                if (result.SubCode == StatusSubCodes.no_quota
                    && result.Type == "num-of-members-quota")
                {
                    throw new WarningMessageException(Resources.Messages.AdobeConnectMeetingHostQuota);
                }

                string msg = string.Format("[AdobeConnectProxy Error] {0}. PrincipalId:{1}.TypeName:{2}.",
                    result.GetErrorInfo(),
                    principalId,
                    typeName);
                _logger.Error(msg);

                throw new AdobeConnectException(result);
            }

            return result;
        }



        public CancelRecordingJobResult CancelRecordingJob(string jobRecordingScoId)
        {
            return Execute(() => { return _provider.CancelRecordingJob(jobRecordingScoId); },
                jobRecordingScoId);
        }

        public ScoInfoResult CreateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
                throw new ArgumentNullException("scoUpdateItem");

            return _provider.CreateSco(scoUpdateItem);
            //return Execute(() => { return _provider.CreateSco(scoUpdateItem); },
            //    scoUpdateItem.Name, scoUpdateItem.FolderId);
        }

        public ScoInfoResult SeminarSessionScoUpdate(SeminarSessionScoUpdateItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return Execute(() => { return _provider.SeminarSessionScoUpdate(item); },
                item.ScoId, item.Name);
        }

        public StatusInfo DeleteSco(string scoId)
        {
            return Execute(() => { return _provider.DeleteSco(scoId); },
                scoId);
        }

        public FieldCollectionResult GetAclFields(int aclId)
        {
            return Execute(() => { return _provider.GetAclFields(aclId); },
                aclId.ToString());
        }

        public PrincipalCollectionResult GetAllByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email can't be empty", "email");

            return Execute(() => { return _provider.GetAllByEmail(email); },
                email);
        }

        public PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails)
        {
            return Execute(() => { return _provider.GetAllByEmail(emails); },
                string.Join(", ", emails));
        }

        public PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm)
        {
            return Execute(() => { return _provider.GetAllByFieldLike(fieldName, searchTerm); },
                fieldName, searchTerm);
        }

        public PrincipalCollectionResult GetAllByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("login can't be empty", "login");

            return Execute(() => { return _provider.GetAllByLogin(login); },
                login);
        }

        public PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins)
        {
            return Execute(() => { return _provider.GetAllByLogin(logins); },
                string.Join(", ", logins));
        }

        public PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind)
        {
            if (principalIdsToFind == null)
                throw new ArgumentNullException("principalIdsToFind");

            return Execute(() => { return _provider.GetAllByPrincipalIds(principalIdsToFind); },
                string.Join(";", principalIdsToFind));
        }

        public PermissionCollectionResult GetAllMeetingEnrollments(string meetingId)
        {
            return Execute(() => { return _provider.GetAllMeetingEnrollments(meetingId); },
                meetingId);
        }

        public PermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds, out bool meetingExistsInAC)
        {
            meetingExistsInAC = true;
            PermissionCollectionResult result;
            try
            {
                result = _provider.GetMeetingPermissions(meetingId, principalIds);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "meetingId:{0}.principalIds:{1}.", meetingId, string.Join(",", principalIds));
                throw new AdobeConnectException("GetMeetingPermissions exception", ex);
            }

            if (!result.Success)
            {
                // TRICK: means No AC Meeting exists
                if (result.Status.Code == StatusCodes.no_access && result.Status.SubCode == StatusSubCodes.denied)
                {
                    string msg = string.Format("[AdobeConnectProxy Error] {0}. Meeting not found. meetingId:{1}. AC: {2}.",
                        result.Status.GetErrorInfo(),
                        meetingId,
                        ApiUrl);
                    _logger.Warn(msg);
                    meetingExistsInAC = false;
                }
                else if (result.Status.Code != StatusCodes.no_data)
                {
                    string msg = string.Format("[AdobeConnectProxy Error] {0}. meetingId:{1}. principalIds: {2}.",
                        result.Status.GetErrorInfo(),
                        meetingId,
                        string.Join(",", principalIds));
                    _logger.Error(msg);
                    throw new AdobeConnectException(result.Status);
                }
            }

            return result;

            //return Execute(() => { return _provider.GetMeetingPermissions(meetingId, principalIds); },
            //    meetingId, string.Join(",", principalIds));
        }

        public PrincipalCollectionResult GetAllPrincipal()
        {
            return Execute(() => { return _provider.GetAllPrincipal(); });
        }

        public PrincipalCollectionResult GetAllPrincipals()
        {
            return Execute(() => { return _provider.GetAllPrincipals(); });
        }

        public CommonInfoResult GetCommonInfo()
        {
            return Execute(() => { return _provider.GetCommonInfo(); });
        }

        public ScoContentCollectionResult GetContentsByScoId(string scoId)
        {
            return Execute(() => { return _provider.GetContentsByScoId(scoId); },
                scoId);
        }

        public ScoContentCollectionResult GetContentsByType(string type)
        {
            return Execute(() => { return _provider.GetContentsByType(type); },
                type);
        }

        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId, string principalId)
        {
            return Execute(() => { return _provider.GetGroupPrincipalUsers(groupId, principalId); },
                groupId, principalId);
        }

        public Tuple<StatusInfo, IEnumerable<Principal>> GetGroupsByType(string type)
        {
            return Execute(() => { return _provider.GetGroupsByType(type); },
                type);
        }

        public Tuple<StatusInfo, IEnumerable<Principal>> GetPrimaryGroupsByType(string type)
        {
            return Execute(() => { return _provider.GetPrimaryGroupsByType(type); },
                type);
        }

        public PrincipalCollectionResult GetGroupUsers(string groupId)
        {
            return Execute(() => { return _provider.GetGroupUsers(groupId); },
                groupId);
        }

        public ScoContentCollectionResult GetMeetingRecordings(IEnumerable<string> scoIds, bool includeMP4recordings = false)
        {
            return Execute(() => { return _provider.GetMeetingRecordings(scoIds, includeMP4recordings); },
                string.Join(";", scoIds), includeMP4recordings.ToString());
        }

        public PrincipalInfoResult GetOneByPrincipalId(string principalId)
        {
            return Execute(() => { return _provider.GetOneByPrincipalId(principalId); },
                principalId);
        }

        public RecordingCollectionResult GetRecordingsList(string folderId)
        {
            return Execute(() => { return _provider.GetRecordingsList(folderId); },
                folderId);
        }

        public ScoInfoResult GetScoByUrl(string scoUrl)
        {
            return Execute(() => { return _provider.GetScoByUrl(scoUrl); }, scoUrl, scoUrl, true);
        }

        public ScoContentResult GetScoContent(string scoId)
        {
            return Execute(() => { return _provider.GetScoContent(scoId); },
                scoId);
        }

        public ScoContentCollectionResult GetScoExpandedContentByName(string scoId, string name)
        {
            return Execute(() => { return _provider.GetScoExpandedContentByName(scoId, name); },
                scoId, name);
        }

        public ScoInfoResult GetScoInfo(string scoId)
        {
            ScoInfoResult result;
            try
            {
                result = _provider.GetScoInfo(scoId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "scoId:{0}.", scoId);
                throw new AdobeConnectException("GetScoInfo exception", ex);
            }

            // TRICK: we process !Success result in our code
            //ResultBase acResult = result as ResultBase;
            //if ((acResult != null) && !acResult.Success && (acResult.Status.Code != StatusCodes.no_data))
            //{
            //    string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
            //        acResult.Status.GetErrorInfo(),
            //        parameterValue);
            //    _logger.Error(msg);
            //    throw new InvalidOperationException(msg);
            //}

            return result;
        }

        public PermissionCollectionResult GetScoPublicAccessPermissions(string scoId)
        {
            return Execute(() => { return _provider.GetScoPublicAccessPermissions(scoId); },
                scoId);
        }

        public PermissionCollectionResult GetScoPermissions(string scoId, string principalId)
        {
            return Execute(() => { return _provider.GetScoPermissions(scoId, principalId); },
                    scoId, principalId, true);
        }

        public ScoShortcut GetShortcutByType(string type)
        {
            StatusInfo status;
            return Execute(() => _provider.GetShortcutByType(type, out status));
        }

        //public UserInfo GetUserInfo(out StatusInfo status)
        //{
        //    UserInfo result;
        //    try
        //    {
        //        result = _provider.GetUserInfo(out status);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("[AdobeConnectProxy Error]", ex);
        //        throw;
        //    }
        //}

        public LoginResult Login(UserCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            return Execute(() => { return _provider.Login(credentials); },
                credentials.Login, credentials.Password, skipAcResultProcessing: true);
        }

        public PrincipalResult PrincipalDelete(PrincipalDelete principalDelete)
        {
            if (principalDelete == null)
                throw new ArgumentNullException("principalDelete");

            return Execute(() => { return _provider.PrincipalDelete(principalDelete); },
                principalDelete.PrincipalId);
        }

        public PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation = false)
        {
            if (principalSetup == null)
                throw new ArgumentNullException("principalSetup");

            return Execute(() => { return _provider.PrincipalUpdate(principalSetup, isUpdateOperation); },
                principalSetup.PrincipalId, principalSetup.Login);
        }

        public PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation, bool throwOnAdobeError)
        {
            if (principalSetup == null)
                throw new ArgumentNullException("principalSetup");

            PrincipalResult result;
            try
            {
                result = _provider.PrincipalUpdate(principalSetup, isUpdateOperation);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "PrincipalId:{0}.Login:{1}.", principalSetup.PrincipalId, principalSetup.Login);
                throw new AdobeConnectException("PrincipalUpdate exception", ex);
            }

            if (!result.Success)
            {
                string msg = string.Format("[AdobeConnectProxy Error] {0}. PrincipalId:{1}.Login:{2}.",
                    result.Status.GetErrorInfo(),
                    principalSetup.PrincipalId,
                    principalSetup.Login);
                _logger.Error(msg);

                if (throwOnAdobeError)
                    throw new AdobeConnectException(result.Status);
            }

            return result;
        }


        public GenericResult PrincipalUpdatePassword(string principalId, string newPassword)
        {
            return Execute(() => { return _provider.PrincipalUpdatePassword(principalId, newPassword); },
                principalId, newPassword);
        }

        public StatusInfo RemoveFromGroup(string principalId, string groupId)
        {
            return Execute(() => { return _provider.RemoveFromGroup(principalId, groupId); }, principalId, groupId);
        }

        public bool RemoveFromGroupByType(string principalId, string typeName)
        {
            return Execute(() => { return _provider.RemoveFromGroupByType(principalId, typeName); }, principalId, typeName);
        }

        public MeetingItemCollectionResult ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMeetingsByName(nameLikeCriteria, startIndex, limit); },
                nameLikeCriteria);
        }

        public TransactionCollectionResult ReportMeetingTransactionsForPrincipal(string principalId, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMeetingTransactionsForPrincipal(principalId, startIndex, limit); },
                principalId);
        }

        public MeetingAttendeeCollectionResult ReportMettingAttendance(string scoId, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMettingAttendance(scoId, startIndex, limit); },
                scoId);
        }

        public MeetingSessionCollectionResult ReportMettingSessions(string scoId, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMettingSessions(scoId, startIndex, limit); },
                scoId);
        }

        public MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMyMeetings(startIndex, limit); });
        }

        public RecordingJobResult ScheduleRecordingJob(string recordingScoId)
        {
            return Execute(() => { return _provider.ScheduleRecordingJob(recordingScoId); },
                recordingScoId);
        }

        public ScoContentCollectionResult SearchScoByName(string name)
        {
            return _provider.SearchScoByName(name);
        }

        public TelephonyProfilesCollectionResult TelephonyProfileList(string principalId)
        {

            TelephonyProfilesCollectionResult result;
            try
            {
                result = _provider.TelephonyProfileList(principalId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProfileList. PrincipalId:{0}", principalId);
                throw new AdobeConnectException("TelephonyProfileList exception", ex);
            }

            return result;
        }

        public TelephonyProfileInfoResult TelephonyProfileInfo(string profileId)
        {
            TelephonyProfileInfoResult result;
            try
            {
                result = _provider.TelephonyProfileInfo(profileId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProfileInfo. ProfileId:{0}", profileId);
                throw;
            }

            return result;
        }

        public StatusInfo UpdateAclField(string aclId, AclFieldId fieldId, string value)
        {
            StatusInfo result;
            try
            {
                result = _provider.UpdateAclField(aclId, fieldId, value);
            }
            catch (Exception ex)
            {
                _logger.Error("[AdobeConnectProxy Error]", ex);
                throw new AdobeConnectException("UpdateAclField exception", ex);
            }

            if (result.Code != StatusCodes.ok)
            {
                string errorInfo = result.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new AdobeConnectException(result);
            }

            return result;
        }

        public StatusInfo UpdatePublicAccessPermissions(string aclId, PermissionId permissionId)
        {
            return Execute(() => { return _provider.UpdatePublicAccessPermissions(aclId, permissionId); },
                aclId, permissionId.ToString());
        }

        public StatusInfo UpdatePublicAccessPermissions(string aclId, SpecialPermissionId permissionId)
        {
            return Execute(() => { return _provider.UpdatePublicAccessPermissions(aclId, permissionId); },
                aclId, permissionId.ToString());
        }

        public ScoInfoResult UpdateSco<T>(T scoUpdateItem) where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
                throw new ArgumentNullException("scoUpdateItem");

            return _provider.UpdateSco(scoUpdateItem);
            //return Execute(() => { return _provider.UpdateSco(scoUpdateItem); },
            //    scoUpdateItem.ScoId, scoUpdateItem.Name);
        }

        public StatusInfo UpdateScoPermissionForPrincipal(IEnumerable<PermissionUpdateTrio> values)
        {
            StatusInfo result;
            try
            {
                result = _provider.UpdateScoPermissionForPrincipal(values);
            }
            catch (Exception ex)
            {
                _logger.Error("[AdobeConnectProxy Error]", ex);
                throw new AdobeConnectException("UpdateScoPermissionForPrincipal exception", ex);
            }

            if (result.Code != StatusCodes.ok)
            {
                string errorInfo = result.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new AdobeConnectException(result);
            }

            return result;
        }

        public StatusInfo UpdateScoPermissionForPrincipal(string scoId, string principalId, MeetingPermissionId permissionId)
        {
            return Execute(() => { return _provider.UpdateScoPermissionForPrincipal(scoId, principalId, permissionId); },
                scoId, principalId);
        }

        public StatusInfo UploadContent(UploadScoInfo uploadScoInfo)
        {
            return Execute(() => { return _provider.UploadContent(uploadScoInfo); }, uploadScoInfo.scoId);
        }

        public byte[] GetContentByUrlPath(string urlPath, string format, out string error)
        {
            //TODO:
            //return Execute(() => { return _provider.GetContentByUrlPath(urlPath, format, out error); },
            //    urlPath, format);

            return _provider.GetContentByUrlPath(urlPath, format, out error);
        }

        public UserCollectionResult ReportGuestsByEmail(string email)
        {
            return Execute(() => { return _provider.ReportGuestsByEmail(email); },
                email);
        }

        public UserCollectionResult ReportGuestsByLogin(string login)
        {
            return Execute(() => { return _provider.ReportGuestsByEmail(login); },
                login);
        }

        private T Execute<T>(Func<T> func)
        {
            T result;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                _logger.Error("[AdobeConnectProxy Error]", ex);
                throw new AdobeConnectException("Execute<T>(Func<T> func) exception", ex);
            }

            if (typeof(T) == typeof(StatusInfo))
            {
                StatusInfo status = result as StatusInfo;
                if (status.Code != StatusCodes.ok)
                {
                    string errorInfo = status.GetErrorInfo();
                    _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                    throw new AdobeConnectException(status);
                }
            }

            ResultBase acResult = result as ResultBase;
            if ((acResult != null) && !acResult.Success)
            {
                string errorInfo = acResult.Status.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new AdobeConnectException(acResult.Status);
            }

            return result;
        }

        private T Execute<T>(Func<T> func, string parameterValue)
        {
            T result;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Parameter1:{0}.", parameterValue);
                throw new AdobeConnectException("Execute<T>(Func<T> func, string parameterValue) exception", ex);
            }

            if (typeof(T) == typeof(StatusInfo))
            {
                StatusInfo status = result as StatusInfo;
                if (status.Code != StatusCodes.ok)
                {
                    string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
                    status.GetErrorInfo(),
                    parameterValue);
                    _logger.Error(msg);
                    throw new AdobeConnectException(status);
                }
            }

            ResultBase acResult = result as ResultBase;
            if ((acResult != null) && !acResult.Success && (acResult.Status.Code != StatusCodes.no_data))
            {
                string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
                    acResult.Status.GetErrorInfo(),
                    parameterValue);
                _logger.Error(msg);
                throw new AdobeConnectException(acResult.Status);
            }

            return result;
        }

        private T Execute<T>(Func<T> func, string parameter1Value, string parameter2Value, bool skipAcResultProcessing = false)
        {
            T result;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Parameter1:{0}.Parameter2:{1}.", parameter1Value, parameter2Value);
                throw new AdobeConnectException("Execute<T>(Func<T> func, string parameter1Value, string parameter2Value, bool skipAcResultProcessing = false) exception", ex);
            }

            if (skipAcResultProcessing)
                return result;

            if (typeof(T) == typeof(StatusInfo))
            {
                StatusInfo status = result as StatusInfo;
                if (status != null
                    && status.Code != StatusCodes.ok
                    && status.SubCode == StatusSubCodes.no_quota
                    && status.Type == "num-of-members-quota")
                {
                    throw new WarningMessageException(Resources.Messages.AdobeConnectMeetingHostQuota);
                }

                if (status.Code != StatusCodes.ok)
                {
                    string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.Parameter2:{2}.",
                    status.GetErrorInfo(),
                    parameter1Value,
                    parameter2Value);
                    _logger.Error(msg);
                    throw new AdobeConnectException(status);
                }
            }

            ResultBase acResult = result as ResultBase;
            if ((acResult != null) && !acResult.Success)
            {
                string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.Parameter2:{2}.",
                    acResult.Status.GetErrorInfo(),
                    parameter1Value,
                    parameter2Value);
                _logger.Error(msg);
                throw new AdobeConnectException(acResult.Status);
            }

            return result;
        }

    }

}
