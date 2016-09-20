using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;

namespace Esynctraining.AdobeConnect
{
    public partial class AdobeConnectProxy : IAdobeConnectProxy
    {
        private readonly AdobeConnectProvider _provider;
        private readonly ILogger _logger;
        private const int ChunkSize = 50;

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

        public StatusInfo AddToGroupByType(IEnumerable<string> principalIds, PrincipalType type)
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
                result = _provider.AddToGroupByType(principalIds, type);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "PrincipalIds:{0}.TypeName:{1}.", string.Join(";", principalIds), type.ToString());
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
                    type.ToString());
                _logger.Error(msg);

                throw new AdobeConnectException(result);
            }

            return result;
        }

        public StatusInfo AddToGroupByType(string principalId, PrincipalType type)
        {
            //return Execute(() => _provider.AddToGroupByType(principalId, typeName),
            //    principalId, typeName);

            StatusInfo result;
            try
            {
                result = _provider.AddToGroupByType(principalId, type);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "PrincipalId:{0}.TypeName:{1}.", principalId, type.ToString());
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
                    type.ToString());
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

        public StatusInfo MoveSco(string folderId, string scoId)
        {
            return Execute(() => { return _provider.MoveSco(folderId, scoId); },
                   folderId, scoId);
        }

        public ScoInfoResult SeminarSessionScoUpdate(SeminarSessionScoUpdateItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return Execute(() => { return _provider.SeminarSessionScoUpdate(item); },
                item.ScoId, item.Name, true);
        }

        public CollectionResult<SharedSeminarLicenseSco> GetSharedSeminarLicenses(string scoId)
        {
            if (string.IsNullOrEmpty(scoId))
                throw new ArgumentException("scoId");

            return Execute(() => { return _provider.GetSharedSeminarLicenses(scoId); },
                scoId);
        }

        public CollectionResult<UserSeminarLicenseSco> GetUserSeminarLicenses(string scoId)
        {
            if (string.IsNullOrEmpty(scoId))
                throw new ArgumentException("scoId");

            return Execute(() => { return _provider.GetUserSeminarLicenses(scoId); },
                scoId);
        }

        public RecordingCollectionResult GetSeminarSessionRecordingsList(string seminarId, string seminarSessionId)
        {
            return Execute(() => { return _provider.GetSeminarSessionRecordingsList(seminarId, seminarSessionId); },
                seminarId, seminarSessionId);
        }

        public StatusInfo DeleteSco(string scoId)
        {
            return Execute(() => { return _provider.DeleteSco(scoId); },
                scoId);
        }

        public FieldCollectionResult GetAclFields(long aclId)
        {
            return Execute(() => { return _provider.GetAclFields(aclId); },
                aclId.ToString());
        }

        public string GetAclField(string scoId, string aclId)
        {
            var result = Execute(() => _provider.GetAclField(scoId, aclId),
                aclId);
            if (!result.Success)
            {
                throw new InvalidOperationException($"Can't get Acl Field by scoId {scoId} and aclId {aclId}");
            }
            return result.FieldValue;
        }

        public CustomField GetCustomField(string name)
        {
            var result =Execute(() => { return _provider.GetCustomField(name); },
                name);
            if (!result.Success)
            {
                throw new InvalidOperationException($"Getting custom field {name} failed {result.Status}");
            }

            var fieldName = result.Value;
            return fieldName;
        }

        public PrincipalCollectionResult GetAllByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email can't be empty", nameof(email));

            return Execute(() => { return _provider.GetAllByEmail(email); },
                email);
        }

        public PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails)
        {
            if (emails == null)
                throw new ArgumentNullException(nameof(emails));

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
                throw new ArgumentException("login can't be empty", nameof(login));

            return Execute(() => { return _provider.GetAllByLogin(login); },
                login);
        }

        public PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins)
        {
            if (logins == null)
                throw new ArgumentNullException(nameof(logins));

            return Execute(() => { return _provider.GetAllByLogin(logins); },
                string.Join(", ", logins));
        }

        public PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind)
        {
            if (principalIdsToFind == null)
                throw new ArgumentNullException(nameof(principalIdsToFind));
            
            var allItems = new List<Principal>();
            StatusInfo status = null;
            foreach (var chunk in principalIdsToFind.Chunk(ChunkSize))
            {
                var chunkResult = Execute(() => _provider.GetAllByPrincipalIds(chunk.ToArray()), string.Join(";", chunk));
                if (!chunkResult.Success)
                {
                    var message = $"provider GetAllByPrincipalIds failed for a chunk of users { string.Join(",", principalIdsToFind) }";
                    throw new InvalidOperationException(message);
                }
                var items = chunkResult.Values;
                allItems.AddRange(items);
                status = chunkResult.Status;
            }
            var result = new PrincipalCollectionResult(status, allItems);
            return result;
        }

        public PrincipalInfoResult GetPrincipalInfo(string principalId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));
            return Execute(() => GetOneByPrincipalId(principalId), principalId);
        }

        public MeetingPermissionCollectionResult GetAllMeetingEnrollments(string meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingId));

            return Execute(() => { return _provider.GetAllMeetingEnrollments(meetingId); },
                meetingId);
        }

        public MeetingPermissionCollectionResult GetMeetingPermissions(string meetingId, IEnumerable<string> principalIds, out bool meetingExistsInAC)
        {
            meetingExistsInAC = true;
            MeetingPermissionCollectionResult result;
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
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetContentsByScoId(scoId); },
                scoId);
        }

        public ScoContentCollectionResult GetContentsByType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Non-empty value expected", nameof(type));

            return Execute(() => { return _provider.GetContentsByType(type); },
                type);
        }

        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId, string principalId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            return Execute(() => { return _provider.GetGroupPrincipalUsers(groupId, principalId); },
                groupId, principalId);
        }

        public PrincipalCollectionResult GetGroupsByType(PrincipalType type)
        {
            return Execute(() => { return _provider.GetGroupsByType(type); },
                type.ToString());
        }

        public PrincipalCollectionResult GetPrimaryGroupsByType(PrincipalType type)
        {
            return Execute(() => { return _provider.GetPrimaryGroupsByType(type); },
                type.ToString());
        }

        public PrincipalCollectionResult GetGroupUsers(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));

            return Execute(() => { return _provider.GetGroupUsers(groupId); },
                groupId);
        }

        public ScoContentCollectionResult GetMeetingRecordings(IEnumerable<string> scoIds, bool includeMP4recordings = false)
        {
            if (scoIds == null)
                throw new ArgumentNullException(nameof(scoIds));

            return Execute(() => { return _provider.GetMeetingRecordings(scoIds, includeMP4recordings); },
                string.Join(";", scoIds), includeMP4recordings.ToString());
        }

        public PrincipalInfoResult GetOneByPrincipalId(string principalId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            return Execute(() => { return _provider.GetOneByPrincipalId(principalId); },
                principalId);
        }

        public RecordingCollectionResult GetRecordingsList(string folderId)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                throw new ArgumentException("Non-empty value expected", nameof(folderId));

            return Execute(() => { return _provider.GetRecordingsList(folderId); },
                folderId);
        }

        public RecordingCollectionResult GetRecordingsList(string folderId, int skip, int take, string propertySortBy, SortOrder order, bool excludeMp4 = false)
        {
            return Execute(() => { return _provider.GetRecordingsList(folderId, skip, take, propertySortBy, order, excludeMp4); },
                folderId);
        }

        public ScoInfoResult GetScoByUrl(string scoUrl)
        {
            return Execute(() => { return _provider.GetScoByUrl(scoUrl); }, scoUrl, scoUrl, true);
        }

        public ScoContentResult GetScoContent(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoContent(scoId); },
                scoId);
        }
        public ScoContentCollectionResult GetScoExpandedContent(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoExpandedContent(scoId); },
                scoId);
        }

        public ScoContentCollectionResult GetScoExpandedContentByName(string scoId, string name)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoExpandedContentByName(scoId, name); },
                scoId, name);
        }

        public ScoContentCollectionResult GetScoExpandedContentByNameLike(string scoId, string nameLikeCriteria)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoExpandedContentByNameLike(scoId, nameLikeCriteria); },
                scoId, nameLikeCriteria);
        }

        public ScoInfoResult GetScoInfo(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

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
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoPublicAccessPermissions(scoId); },
                scoId);
        }

        /// <summary>
        /// Returns permissions for SCO (SCOs other than meetings or courses, e.g. files\folders)
        /// Returns only records with view\publish\manage\denied permissions.
        /// </summary>
        public PermissionCollectionResult GetScoPermissions(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return Execute(() => { return _provider.GetScoPermissions(scoId); },
                    scoId);
        }

        public PermissionCollectionResult GetScoPermissions(string scoId, string principalId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            return Execute(() => { return _provider.GetScoPermissions(scoId, principalId); },
                    scoId, principalId, true);
        }

        public IEnumerable<ScoShortcut> GetShortcuts()
        {
            StatusInfo status;
            return Execute(() => _provider.GetShortcuts(out status));
        }

        public ScoShortcut GetShortcutByType(string type)
        {
            StatusInfo status;
            return Execute(() => _provider.GetShortcutByType(type, out status));
        }

        public ScoShortcut GetShortcutByType(string type, out StatusInfo status)
        {
            StatusInfo st = null;
            var result = Execute(() =>
            {
                return _provider.GetShortcutByType(type, out st);
            });

            status = st;
            return result;
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
                throw new ArgumentNullException(nameof(credentials));

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

        public bool RemoveFromGroupByType(string principalId, PrincipalType type)
        {
            return Execute(() => { return _provider.RemoveFromGroupByType(principalId, type); }, principalId, type.ToString());
        }

        public MeetingItemCollectionResult ReportMeetingsByName(string nameLikeCriteria, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMeetingsByName(nameLikeCriteria, startIndex, limit); },
                nameLikeCriteria);
        }

        public TransactionCollectionResult ReportRecordingTransactions(IEnumerable<string> recordingScoIdList, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportRecordingTransactions(recordingScoIdList, startIndex, limit); },
                   string.Join(",", recordingScoIdList));
        }

        public TransactionCollectionResult ReportMeetingTransactionsForPrincipal(string principalId, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMeetingTransactionsForPrincipal(principalId, startIndex, limit); },
                principalId);
        }

        public MeetingAttendeeCollectionResult ReportMeetingAttendance(string scoId, int startIndex = 0, int limit = 0, bool returnCurrentUsers = false)
        {
            return Execute(() => { return _provider.ReportMeetingAttendance(scoId, startIndex, limit, returnCurrentUsers); },
                scoId);
        }

        public MeetingSessionCollectionResult ReportMeetingSessions(string scoId, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMettingSessions(scoId, startIndex, limit); },
                scoId);
        }

        public MeetingItemCollectionResult ReportMyMeetings(int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMyMeetings(startIndex, limit); });
        }

        public MeetingItemCollectionResult ReportMyMeetings(MeetingPermissionId permission, int startIndex = 0, int limit = 0)
        {
            return Execute(() => { return _provider.ReportMyMeetings(permission, startIndex, limit); });
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

        public StatusInfo UpdateScoPermissions(IEnumerable<IPermissionUpdateTrio> values)
        {
            StatusInfo result;
            try
            {
                result = _provider.UpdateScoPermissions(values);
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
        
        public StatusInfo UploadContent(UploadScoInfo uploadScoInfo)
        {
            return Execute(() => { return _provider.UploadContent(uploadScoInfo); }, uploadScoInfo.scoId);
        }

        public byte[] GetContent(string scoId, out string error, string format = "zip")
        {
            // TODO:
            return _provider.GetContent(scoId, out error, format);
        }

        public byte[] GetSourceContent(string urlPath, out string error, string format = "zip")
        {
            return _provider.GetSourceContentByUrlPath2(urlPath, format, out error);
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
