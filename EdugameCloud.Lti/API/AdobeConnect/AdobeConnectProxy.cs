using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using EdugameCloud.Lti.Core;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class AdobeConnectProxy : IAdobeConnectProxy
    {
        private readonly AdobeConnectProvider _provider;
        private readonly ILogger _logger;
        // TRICK: for logging only
        private readonly string _apiUrl;


        public AdobeConnectProxy(AdobeConnectProvider provider, ILogger logger, string apiUrl)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (logger == null)
                throw new ArgumentNullException("logger");

            _provider = provider;
            _logger = logger;
            _apiUrl = apiUrl;
        }


        public StatusInfo AddToGroupByType(IEnumerable<string> principalIds, string typeName)
        {
            if (principalIds == null)
                throw new ArgumentNullException("principalIds");

            return Execute(() => _provider.AddToGroupByType(principalIds, typeName),
                string.Join(";", principalIds), typeName);
        }

        public StatusInfo AddToGroupByType(string principalId, string typeName)
        {
            return Execute(() => _provider.AddToGroupByType(principalId, typeName), 
                principalId, typeName);
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
            return Execute(() => { return _provider.GetAllByEmail(email); },
                email);
        }

        public PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm)
        {
            return Execute(() => { return _provider.GetAllByFieldLike(fieldName, searchTerm); },
                fieldName, searchTerm);
        }

        public PrincipalCollectionResult GetAllByLogin(string login)
        {
            return Execute(() => { return _provider.GetAllByLogin(login); },
                login);
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

        public Tuple<StatusInfo, IEnumerable<Principal>> GetGroupsByType(string type)
        {
            return Execute(() => { return _provider.GetGroupsByType(type); },
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
                throw;
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

        public ScoShortcut GetShortcutByType(string type, out StatusInfo status)
        {
            ScoShortcut result;
            try
            {
                result = _provider.GetShortcutByType(type, out status);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("[AdobeConnectProxy Error]", ex);
                throw;
            }
        }

        public UserInfo GetUserInfo(out StatusInfo status)
        {
            UserInfo result;
            try
            {
                result = _provider.GetUserInfo(out status);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("[AdobeConnectProxy Error]", ex);
                throw;
            }
        }

        public LoginResult Login(UserCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            return Execute(() => { return _provider.Login(credentials); },
                credentials.Login, credentials.Password);
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
                throw;
            }

            if (!result.Success)
            {
                string msg = string.Format("[AdobeConnectProxy Error] {0}. PrincipalId:{1}.Login:{2}.",
                    result.Status.GetErrorInfo(),
                    principalSetup.PrincipalId, 
                    principalSetup.Login);
                _logger.Error(msg);

                if (throwOnAdobeError)
                    throw new InvalidOperationException(msg);
            }

            return result;
        }


        public GenericResult PrincipalUpdatePassword(string principalId, string newPassword)
        {
            return Execute(() => { return _provider.PrincipalUpdatePassword(principalId, newPassword); },
                principalId, newPassword);
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

        public RecordingJobResult ScheduleRecordingJob(string recordingScoId)
        {
            return Execute(() => { return _provider.ScheduleRecordingJob(recordingScoId); },
                recordingScoId);
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
                throw;
            }

            if (result.Code != StatusCodes.ok)
            {
                string errorInfo = result.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new InvalidOperationException(errorInfo);
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
                throw;
            }

            if (result.Code != StatusCodes.ok)
            {
                string errorInfo = result.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new InvalidOperationException(errorInfo);
            }

            return result;
        }

        public StatusInfo UpdateScoPermissionForPrincipal(string scoId, string principalId, MeetingPermissionId permissionId)
        {
            return Execute(() => { return _provider.UpdateScoPermissionForPrincipal(scoId, principalId, permissionId); },
                scoId, principalId);
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
                throw;
            }

            if (typeof(T) == typeof(StatusInfo))
            {
                StatusInfo status = result as StatusInfo;
                if (status.Code != StatusCodes.ok)
                {
                    string errorInfo = status.GetErrorInfo();
                    _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                    throw new InvalidOperationException(errorInfo);
                }
            }

            ResultBase acResult = result as ResultBase;
            if ((acResult != null) && !acResult.Success)
            {
                string errorInfo = acResult.Status.GetErrorInfo();
                _logger.Error("[AdobeConnectProxy Error] " + errorInfo);
                throw new InvalidOperationException(errorInfo);
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
                throw;
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
                    throw new InvalidOperationException(msg);
                }
            }

            ResultBase acResult = result as ResultBase;
            if ((acResult != null) && !acResult.Success && (acResult.Status.Code != StatusCodes.no_data))
            {
                string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
                    acResult.Status.GetErrorInfo(),
                    parameterValue);
                _logger.Error(msg);
                throw new InvalidOperationException(msg);
            }

            return result;
        }

        private T Execute<T>(Func<T> func, string parameter1Value, string parameter2Value)
        {
            T result;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Parameter1:{0}.Parameter2:{1}.", parameter1Value, parameter2Value);
                throw;
            }

            if (typeof(T) == typeof(StatusInfo))
            {
                StatusInfo status = result as StatusInfo;
                if (status != null 
                    && status.Code != StatusCodes.ok
                    && status.SubCode == StatusSubCodes.no_quota
                    && status.Type == "num-of-members-quota")
                {
                    throw new WarningMessageException("You have exceeded the number of meeting hosts for your Adobe Connect account.  Please consider adding additional meeting hosts or remove meeting hosts that are inactive.");
                }

                if (status.Code != StatusCodes.ok)
                {
                    string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.Parameter2:{2}.",
                    status.GetErrorInfo(),
                    parameter1Value,
                    parameter2Value);
                    _logger.Error(msg);
                    throw new InvalidOperationException(msg);
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
                throw new InvalidOperationException(msg);
            }

            return result;
        }

    }

}
