using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class RecordingsService : IRecordingsService
    {
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly LmsUserModel lmsUserModel;
        private readonly IAdobeConnectAccountService acAccountService;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly ILogger logger;


        public RecordingsService(LmsCourseMeetingModel lmsCourseMeetingModel, LmsUserModel lmsUserModel,
            IAdobeConnectAccountService acAccountService, MeetingSetup meetingSetup, UsersSetup usersSetup, ILogger logger)
        {
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.lmsUserModel = lmsUserModel;
            this.acAccountService = acAccountService;
            this.meetingSetup = meetingSetup;
            this.logger = logger;
            this.usersSetup = usersSetup;
        }


        public IEnumerable<IRecordingDto> GetRecordings(LmsCompany lmsCompany, Esynctraining.AdobeConnect.IAdobeConnectProxy provider, 
            int courseId, 
            int id,
            Func<IRoomTypeFactory> getRoomTypeFactory)
        {
            LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, courseId, id);

            if (meeting == null)
            {
                return Enumerable.Empty<RecordingDTO>();
            }

            var timeZone = acAccountService.GetAccountDetails(provider, IoC.Resolve<ICache>()).TimeZoneInfo;

            IEnumerable<IRecordingDto> result;
            var meetingSco = meeting.GetMeetingScoId();
            var commonInfo = provider.GetCommonInfo().CommonInfo;
            if (commonInfo.MajorVersion <= 9 && commonInfo.MinorVersion < 1)
            {
                result = GetRecordingsLegacy(meetingSco, commonInfo.AccountUrl, timeZone, provider);
            }

            var factory = getRoomTypeFactory();
            var recordingsExtractor = factory.GetRecordingExtractor();
            result = recordingsExtractor.GetRecordings(factory.GetRecordingDtoBuilder(), meetingSco, commonInfo.AccountUrl, timeZone);

            ProcessPublishedFlag(lmsCompany, meeting, result);

            return result;
        }

        //private static List<RecordingDTO> GetRecordings(string meetingSco, 
        //    string accountUrl,
        //    TimeZoneInfo timeZone,
        //    Esynctraining.AdobeConnect.IAdobeConnectProxy provider)
        //{
        //    var result = new List<RecordingDTO>();
        //    var apiRecordings = provider.GetRecordingsList(meetingSco);
        //    foreach (var v in apiRecordings.Values)
        //    {
        //        var moreDetails = provider.GetScoPublicAccessPermissions(v.ScoId);
        //        var isPublic = false;
        //        if (moreDetails.Success && moreDetails.Values.Any())
        //        {
        //            isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
        //        }

        //        // NOTE: not in use on client-site
        //        //string passcode = provider.GetAclField(v.ScoId, AclFieldId.meeting_passcode).FieldValue;

        //        result.Add(new RecordingDTO(v, accountUrl, timeZone)
        //        {
        //            IsPublic = isPublic,

        //        });
        //    }

        //    return result;
        //}

        private static List<RecordingDTO> GetRecordingsLegacy(string meetingSco, 
            string accountUrl,
            TimeZoneInfo timeZone,
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider)
        {
            var result = new List<RecordingDTO>();
            ScoContentCollectionResult apiRecordings = provider.GetMeetingRecordings(new [] {meetingSco});

            foreach (var v in apiRecordings.Values)
            {
                var moreDetails = provider.GetScoPublicAccessPermissions(v.ScoId);
                var isPublic = false;
                if (moreDetails.Success && moreDetails.Values.Any())
                {
                    isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
                }

                result.Add(new RecordingDTO(v, accountUrl, isPublic, timeZone));
            }

            return result;
        }

        private static void ProcessPublishedFlag(LmsCompany lmsCompany, LmsCourseMeeting meeting, IEnumerable<IRecordingDto> records)
        {
            if (lmsCompany.AutoPublishRecordings)
            {
                foreach (var item in records)
                    item.Published = true;
            }
            else
            {
                foreach (var item in records)
                    item.Published = meeting.MeetingRecordings.Any(r => r.ScoId == item.Id);
            }
        }

        public string UpdateRecording(LmsCompany lmsCompany, IAdobeConnectProxy provider, string id, bool isPublic, string password)
        {
            var recording = provider.GetScoInfo(id).ScoInfo;

            if (recording == null)
            {
                return string.Empty;
            }

            // ReSharper disable UnusedVariable
            var accessResult = provider.UpdatePublicAccessPermissions(id, isPublic ? PermissionId.view : PermissionId.remove);
            var passwordResult = provider.UpdateAclField(id, AclFieldId.meeting_passcode, password);
            // ReSharper restore UnusedVariable
            var recordingUrl = (lmsCompany.AcServer.EndsWith("/")
                ? lmsCompany.AcServer.Substring(0, lmsCompany.AcServer.Length - 1)
                : lmsCompany.AcServer) + recording.UrlPath;

            return recordingUrl;
        }

        public string JoinRecording(LmsCompany lmsCompany, LtiParamDTO param, string recordingUrl,
            ref string breezeSession, string mode = null, IAdobeConnectProxy adobeConnectProvider = null)
        {
            var breezeToken = string.Empty;
            
            IAdobeConnectProxy provider = adobeConnectProvider ?? acAccountService.GetProvider(lmsCompany);
            LmsUserDTO lmsUserDto = null;

            var acRecordingScoResult = provider.GetScoByUrl(recordingUrl);
            if (!acRecordingScoResult.Success)
            {
                logger.Error(string.Format("[AdobeConnectProxy Error] {0}. Recording url:{1}", (object)StatusInfoExtentions.GetErrorInfo(acRecordingScoResult.Status), recordingUrl));
                throw new AdobeConnectException(acRecordingScoResult.Status);
            }

            var courseMeetings = this.lmsCourseMeetingModel.GetByCompanyAndScoId(lmsCompany, acRecordingScoResult.ScoInfo.FolderId, 0);

            var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                throw new Core.WarningMessageException(string.Format("No user with id {0} found in the database.",
                    param.lms_user_id));
            }

            // for AC provisioning courseMeeting.scoId() is used, so here we can take only first record if Dynamic provisioning is enabled for at least one meeting 
            // if sync is enabled then all meetings with the same scoId should have the same users roster and the same value of EnableDynamicProvisioning property
            var courseMeeting = courseMeetings.FirstOrDefault(x => x.EnableDynamicProvisioning);
            if (courseMeeting != null && (LmsMeetingType)courseMeeting.LmsMeetingType != LmsMeetingType.StudyGroup && lmsCompany.UseSynchronizedUsers)
            {
                string userCreationError = null;
                lmsUserDto = usersSetup.GetOrCreateUserWithAcRole(lmsCompany, provider, param, courseMeeting, out userCreationError, param.lms_user_id);
                if (userCreationError != null)
                {
                    throw new Core.WarningMessageException(
                        string.Format(
                            "[Dynamic provisioning] Could not create user, id={0}. Message: {1}",
                            param.lms_user_id, userCreationError));
                }

                meetingSetup.ProcessDynamicProvisioning(provider, lmsCompany, courseMeeting, lmsUser, lmsUserDto);
            }

            var principalInfo = lmsUser.PrincipalId != null
                ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo
                : null;
            var registeredUser = principalInfo != null ? principalInfo.Principal : null;

            if (registeredUser != null)
            {
                meetingSetup.ProcessGuestAuditUsers(provider, lmsCompany, acRecordingScoResult.ScoInfo.FolderId,
                    registeredUser.PrincipalId, param, () => courseMeetings);
                breezeToken = meetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, provider);
            }
            else
            {
                throw new Core.WarningMessageException(string.Format(
                    "No user with principal id {0} found in Adobe Connect.", lmsUser.PrincipalId ?? string.Empty));
            }

            var baseUrl = lmsCompany.AcServer + "/" + recordingUrl;

            breezeSession = breezeToken ?? string.Empty;

            return string.Format(
                "{0}?{1}{2}{3}",
                baseUrl,
                mode != null ? string.Format("pbMode={0}", mode) : string.Empty,
                mode != null && !lmsCompany.LoginUsingCookie.GetValueOrDefault() ? "&" : string.Empty,
                !lmsCompany.LoginUsingCookie.GetValueOrDefault() && breezeToken != null
                    ? "session=" + breezeToken
                    : string.Empty);
        }

        public OperationResult EditRecording(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id,
            string name, 
            string summary)
        {
            LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, courseId, id);

            if (meeting == null)
            {
                return OperationResult.Error(Resources.Messages.MeetingNotFound);
            }

            ScoContentCollectionResult result = provider.GetMeetingRecordings(new[] { meeting.GetMeetingScoId() }, lmsCompany.UseMP4);

            var recording = result.Values.FirstOrDefault(x => x.ScoId == recordingId);

            if (recording == null)
            {
                return OperationResult.Error(Resources.Messages.RecordingNotFound);
            }
            
            ScoInfoResult editResult = provider.UpdateSco<RecordingUpdateItem>(new RecordingUpdateItem
            {
                ScoId = recordingId,
                Name = name.Trim(),
                Description = summary,
                Type = ScoType.content,
            });

            if (!editResult.Success || editResult.ScoInfo == null)
            {
                if ((editResult.Status.SubCode == StatusSubCodes.duplicate) && (editResult.Status.InvalidField == "name"))
                    return OperationResult.Error(Resources.Messages.NotUniqueName);

                return OperationResult.Error(editResult.Status.GetErrorInfo());
            }

            return OperationResult.Success();
        }

        public OperationResult RemoveRecording(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id)
        {
            LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, courseId, id);

            if (meeting == null)
            {
                return OperationResult.Error(Resources.Messages.MeetingNotFound);
            }

            ScoContentCollectionResult result = provider.GetMeetingRecordings(new[] { meeting.GetMeetingScoId() }, lmsCompany.UseMP4);

            var recording = result.Values.FirstOrDefault(x => x.ScoId == recordingId);

            if (recording == null)
            {
                return OperationResult.Error(Resources.Messages.RecordingNotFound);
            }

            /*if (recording.Icon == "mp4-archive")
            {
                var scheduledRecording = this.GetScheduledRecording(recordingId, scoId, provider);
                if (scheduledRecording.JobStatus == "job-pending")
                {
                    return OperationResult.Error("Cannot delete "  + scheduledRecording.Name  + " MP4 recording. Recording converting - in progress");
                }
            }*/

            provider.DeleteSco(recordingId);
            return OperationResult.Success();
        }

    }

}