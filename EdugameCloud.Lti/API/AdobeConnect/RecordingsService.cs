using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class RecordingsService : IRecordingsService
    {
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly LmsUserModel lmsUserModel;
        private readonly IAdobeConnectAccountService acAccountService;
        private readonly IMeetingSetup meetingSetup;


        public RecordingsService(LmsCourseMeetingModel lmsCourseMeetingModel, LmsUserModel lmsUserModel,
            IAdobeConnectAccountService acAccountService, IMeetingSetup meetingSetup)
        {
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.lmsUserModel = lmsUserModel;
            this.acAccountService = acAccountService;
            this.meetingSetup = meetingSetup;
        }


        public IEnumerable<RecordingDTO> GetRecordings(LmsCompany lmsCompany, IAdobeConnectProxy provider, int courseId, int id)
        {
            LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, courseId, id);

            if (meeting == null)
            {
                return Enumerable.Empty<RecordingDTO>();
            }

            List<RecordingDTO> result;
            var meetingSco = meeting.GetMeetingScoId();
            var commonInfo = provider.GetCommonInfo().CommonInfo;
            if (commonInfo.MajorVersion <= 9 && commonInfo.MinorVersion < 1)
            {
                result = GetRecordingsLegacy(meetingSco, commonInfo.AccountUrl, provider);
            }

            result = GetRecordings(meetingSco, commonInfo.AccountUrl, provider);

            ProcessPublishedFlag(lmsCompany, meeting, result);

            return result;
        }

        private static List<RecordingDTO> GetRecordings(string meetingSco, string accountUrl, IAdobeConnectProxy provider)
        {
            var result = new List<RecordingDTO>();
            var apiRecordings = provider.GetRecordingsList(meetingSco);
            foreach (var v in apiRecordings.Values)
            {
                var moreDetails = provider.GetScoPublicAccessPermissions(v.ScoId);
                var isPublic = false;
                if (moreDetails.Success && moreDetails.Values.Any())
                {
                    isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
                }

                // NOTE: not in use on client-site
                //string passcode = provider.GetAclField(v.ScoId, AclFieldId.meeting_passcode).FieldValue;

                result.Add(new RecordingDTO(v, accountUrl)
                {
                    is_public = isPublic,

                });
            }

            return result;
        }

        private static List<RecordingDTO> GetRecordingsLegacy(string meetingSco, string accountUrl, IAdobeConnectProxy provider)
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

                result.Add(new RecordingDTO(v, accountUrl, isPublic));
            }

            return result;
        }

        private static void ProcessPublishedFlag(LmsCompany lmsCompany, LmsCourseMeeting meeting, List<RecordingDTO> records)
        {
            if (lmsCompany.AutoPublishRecordings)
            {
                records.ForEach(x => x.published = true);
            }
            else
            {
                records.ForEach(x => x.published = meeting.MeetingRecordings.Any(r => r.ScoId == x.id));
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

            var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                throw new WarningMessageException(string.Format("No user with id {0} found in the database.",
                    param.lms_user_id));
            }

            var principalInfo = lmsUser.PrincipalId != null
                ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo
                : null;
            var registeredUser = principalInfo != null ? principalInfo.Principal : null;

            if (registeredUser != null)
            {
                breezeToken = meetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, provider);
            }
            else
            {
                throw new WarningMessageException(string.Format(
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