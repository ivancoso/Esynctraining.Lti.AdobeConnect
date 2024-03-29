﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using EdugameCloud.Core;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using EdugameCloud.Lti.Models;
using EdugameCloud.Lti.Telephony;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed partial class MeetingSetup : IMeetingSetup
    {
        private class MeetingInfo
        {
            public LmsCourseMeeting DbRecord { get; set; }

            public ScoInfo Sco { get; set; }

            public IEnumerable<MeetingPermissionInfo> Permissions { get; set; }

            public TelephonyProfileInfoResult AudioProfile { get; set; }

            public string GetPublicAccessPermission()
            {
                // TRICK:  PermissionStringValue can contains specialpermission value
                // denied!! not in MeetingPermissionId enum
                MeetingPermissionInfo publicAccessPermission = Permissions?.FirstOrDefault(x => x.PrincipalId == "public-access" && !string.IsNullOrWhiteSpace(x.PermissionStringValue));

                return publicAccessPermission != null ? publicAccessPermission.PermissionStringValue : MeetingPermissionId.remove.ToString();
            }

        }

        // TODO:
        public class LoginResult
        {
            public string BreezeSession { get; set; }

            public Principal User { get; set; }
        }

        private static readonly string AcDateFormat = "yyyy-MM-ddTHH:mm"; // AdobeConnectProviderConstants.DateFormat

        #region Properties

        private IJsonSerializer JsonSerializer => IoC.Resolve<IJsonSerializer>();

        private IJsonDeserializer JsonDeserializer => IoC.Resolve<IJsonDeserializer>();

        private IMeetingNameFormatterFactory MeetingNameFormatterFactory => IoC.Resolve<IMeetingNameFormatterFactory>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private OfficeHoursModel OfficeHoursModel => IoC.Resolve<OfficeHoursModel>();

        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        private LmsCompanyModel LmsСompanyModel => IoC.Resolve<LmsCompanyModel>();

        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();

        private LmsFactory LmsFactory => IoC.Resolve<LmsFactory>();

        private LmsProviderModel LmsProviderModel => IoC.Resolve<LmsProviderModel>();

        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();

        private IAdobeConnectUserService AcUserService => IoC.Resolve<IAdobeConnectUserService>();

        private IAdobeConnectAccountService AcAccountService => IoC.Resolve<IAdobeConnectAccountService>();

        private IAudioProfilesService AudioProfileService => IoC.Resolve<IAudioProfilesService>();

        private ISynchronizationUserService SynchronizationUserService => IoC.Resolve<ISynchronizationUserService>();

        private ILogger Logger => IoC.Resolve<ILogger>();
        private dynamic Settings => IoC.Resolve<ApplicationSettingsProvider>();

        #endregion

        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public async Task<OperationResult> DeleteMeetingAsync(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id,
            bool softDelete = false)
        {
            if (!lmsLicense.CanRemoveMeeting.GetValueOrDefault())
            {
                return OperationResult.Error(Resources.Messages.MeetingDeletionDisabled);
            }

            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsLicense.Id, param.course_id, id);
            if (meeting == null)
            {
                return OperationResult.Error(Resources.Messages.MeetingNotFound);
            }

            //if ((meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours) && softDelete)
            //{
            //    var coursesWithThisOfficeHours = this.LmsCourseMeetingModel.GetAllByOfficeHoursId(meeting.OfficeHours.Id);
            //    if (coursesWithThisOfficeHours.Any(c => c.Id != meeting.Id))
            //    {
            //        this.LmsCourseMeetingModel.RegisterDelete(meeting);
            //        return OperationResult.Success();
            //    }
            //}

            //TRICK: before deletion
            //softDelete - means that we should not delete meeting from AC even in case when it is not reused by any other meeting
            string meetingScoId = meeting.GetMeetingScoId();
            //bool acMeetingIsStillUsed =
            //    lmsLicense.EnableMeetingReuse && 
            //        (softDelete || 
            //            this.LmsCourseMeetingModel.ContainsByCompanyAndScoId(lmsLicense, meetingScoId, meeting.Id));
            bool skipRemovingFromAC = softDelete; 

            var meetingSessionService =
                LmsFactory.GetMeetingSessionService(lmsLicense, param);

            await meetingSessionService.DeleteMeetingSessionsAsync(meeting, param);
            await RemoveLmsCalendarEventForMeeting(lmsLicense, meeting);

            this.LmsCourseMeetingModel.RegisterDelete(meeting, flush: true);
                        
            if (skipRemovingFromAC)
            {
                return OperationResult.Success();
            }
            
            // TRICK: remove ALL(all cources \ all licenses) references in DB if we are going to delete from AC!!
            var meetings = this.LmsCourseMeetingModel.GetByCompanyAndScoId(lmsLicense, meetingScoId, meeting.Id);
            if (meetings.Any())
            {
                foreach (var m in meetings)
                {
                    await meetingSessionService.DeleteMeetingSessionsAsync(m, param);
                    this.LmsCourseMeetingModel.RegisterDelete(m, flush: false);
                }
                this.LmsCourseMeetingModel.Flush();
            }

            if (meeting.OfficeHours != null)
            {
                // TODO: test OH delete
                this.OfficeHoursModel.RegisterDelete(meeting.OfficeHours, flush: true);
            }


            var result = provider.DeleteSco(meetingScoId);
            if (result.Code == StatusCodes.ok)
            {
                DeleteAudioProfile(lmsLicense, meeting, provider);

                return OperationResult.Success();
            }

            return OperationResult.Error(result.InnerXml);
        }

        public async Task<IEnumerable<MeetingDTO>> GetMeetingsAsync(ILmsLicense lmsLicense, IAdobeConnectProxy provider,
            LmsUser lmsUser, LtiParamDTO param, StringBuilder trace, bool apiCall = false)
        {
            if (lmsLicense == null)
                throw new ArgumentNullException(nameof(lmsLicense));
            if (lmsUser == null)
                throw new ArgumentNullException(nameof(lmsUser));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            var ret = new List<MeetingDTO>();
            var sw = Stopwatch.StartNew();
            var meetings = this.LmsCourseMeetingModel.GetAllByCourseId(lmsLicense.Id, param.course_id).ToList();
            sw.Stop();

            bool isTeacher = this.UsersSetup.IsTeacher(param, lmsLicense);
            if (!isTeacher && lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.UseCourseSections) && !apiCall)
            {
                LmsCourseSectionsServiceBase sectionsService = LmsFactory.GetCourseSectionsService(lmsLicense.LmsProviderId, lmsLicense.GetLMSSettings(Settings), param);
                var sections = await sectionsService.GetCourseSections();
                meetings =
                    meetings.Where(
                        x =>
                            (LmsMeetingType)x.LmsMeetingType != LmsMeetingType.Meeting
                            || !x.CourseSections.Any()
                            || x.CourseSections.Any(cs => sections.Any(s => s.Id == cs.LmsId && s.Users.Any(u => u.Id == lmsUser.UserId)))).ToList();
            }

            trace?.AppendFormat("\t GetMeetings - LmsCourseMeetingModel.GetAllByCourseId time: {0}\r\n", sw.Elapsed.ToString());

            // NOTE: add office hours meeting, if it exists for the user, but not in current course
            bool addedOfficeHoursFromOtherCourse = false;
            if (lmsLicense.EnableOfficeHours.GetValueOrDefault() 
                && !meetings.Any(m => m.LmsMeetingType == (int)LmsMeetingType.OfficeHours && m.OfficeHours.LmsUser.Id == lmsUser.Id)
                && !apiCall)
            {
                sw = Stopwatch.StartNew();

                var officeHoursMeeting =
                    this.LmsCourseMeetingModel.GetOneByUserAndType(lmsLicense.Id, param.lms_user_id, LmsMeetingType.OfficeHours).Value; //works like FirstOrDefault, so not changing

                if (officeHoursMeeting == null)
                {
                    var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                    if (officeHours != null)
                    {
                        officeHoursMeeting = new LmsCourseMeeting
                        {
                            OfficeHours = officeHours,
                            LmsMeetingType = (int)LmsMeetingType.OfficeHours,
                            LmsCompanyId = lmsLicense.Id,
                            CourseId = param.course_id,
                        };
                    }
                }

                if (officeHoursMeeting != null)
                {
                    meetings.Add(officeHoursMeeting);
                    addedOfficeHoursFromOtherCourse = true;
                }

                sw.Stop();
                trace?.AppendFormat("\t GetMeetings - Get User's Office Hours time: {0}\r\n", sw.Elapsed.ToString());
            }

            sw = Stopwatch.StartNew();

            // TRICK: not to lazy load within Parallel.ForEach
            var sett = lmsLicense.Settings.ToList();
            var resultCollection = new List<MeetingInfo>();
            object localLockObject = new object();

            var lmsUserPrincipalId = lmsUser.PrincipalId;
            var principalsToCheckPermissions = new List<string> { "public-access" };
            if (!string.IsNullOrWhiteSpace(lmsUser.PrincipalId))
                principalsToCheckPermissions.Add(lmsUser.PrincipalId);

            var lmsCompanyId = lmsLicense.Id;
            // TRICK: not to have DB calls from Parallel.ForEach 
            var input = meetings.Select(x => new Tuple<LmsCourseMeeting, string>(x, x.GetMeetingScoId())).ToList();
            Parallel.ForEach<Tuple<LmsCourseMeeting, string>, List<MeetingInfo>>(
                  input,
                  () => new List<MeetingInfo>(),
                  (meeting, state, localList) =>
                  {
                      MeetingInfo info = GetAcMeetingInfo(
                        provider,
                        principalsToCheckPermissions,
                        lmsCompanyId,
                        meeting.Item2,
                        meeting.Item1,
                        trace);
                      localList.Add(info);
                      return localList;
                  },
                  (finalResult) =>
                  {
                      lock (localLockObject)
                          resultCollection.AddRange(finalResult);
                  }
            );

            sw.Stop();
            trace?.AppendFormat("\t GetMeetings - Parallel.ForEach get data from AC ({0} meetings): {1}\r\n", meetings.Count, sw.Elapsed.ToString());

            //TimeZoneInfo timeZone = AcAccountService.GetAccountDetails(provider, IoC.Resolve<ICache>()).TimeZoneInfo;
            sw = Stopwatch.StartNew();
            foreach (MeetingInfo meeting in resultCollection)
            {
                MeetingDTO dto = await BuildDto(
                        lmsUser,
                        param,
                        lmsLicense,
                        meeting,
                        null,
                        //timeZone,
                        trace);

                if (dto != null)
                    ret.Add(dto);
            }

            sw.Stop();
            trace?.AppendFormat("\t GetMeetings - BuildDto: {0}\r\n", sw.Elapsed.ToString());

            if (addedOfficeHoursFromOtherCourse)
            {
                var ohDto = ret.FirstOrDefault(m => m.Type == (int)LmsMeetingType.OfficeHours && m.IsEditable); //IsEditable means that current user is owner
                // NOTE: can be NULL if OH meeting not found in AC
                if (ohDto != null)
                    ohDto.IsDisabledForThisCourse = true;
            }
            return ret;
        }

        public async Task<(string meetingJoinUrl, string breezeSession)> JoinMeeting(LmsCompany lmsLicense, LtiParamDTO param, int meetingId, IAdobeConnectProxy provider)
        {
            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsLicense.Id, param.course_id, meetingId);

            if (currentMeeting == null)
            {
                throw new WarningMessageException(
                    $"No meeting for course {param.course_id} and id {meetingId} found.");
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();
            var meetingUrl = string.Empty;

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                ScoContent currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = string.Concat(lmsLicense.AcServer.TrimEnd('/'), currentMeetingSco.UrlPath);
                }
            }

            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsLicense.Id).Value;
            if (lmsUser == null)
            {
                throw new Core.WarningMessageException(string.Format("No user with id {0} found.", param.lms_user_id));
            }

            //string email = param.lis_person_contact_email_primary;
            //string login = param.lms_user_login;
            LmsUserDTO lmsUserDto = null;
            if ((LmsMeetingType)currentMeeting.LmsMeetingType != LmsMeetingType.StudyGroup
                && currentMeeting.EnableDynamicProvisioning && lmsLicense.UseSynchronizedUsers)
            {
                //string userCreationError = null;
                //lmsUserDto = UsersSetup.GetOrCreateUserWithAcRole(lmsLicense, provider, param, currentMeeting, out userCreationError, param.lms_user_id);
                
                var rr = await UsersSetup.GetOrCreateUserWithAcRole(lmsLicense, provider, param, currentMeeting, param.lms_user_id);
                lmsUserDto = rr.Data;
                //lmsUserDto = UsersSetup.GetOrCreateUserWithAcRole(lmsLicense, provider, param, currentMeeting, out userCreationError, param.lms_user_id);

                if (!rr.IsSuccess)
                {
                    throw new Core.WarningMessageException(
                        string.Format(
                            "[Dynamic provisioning] Could not create user, id={0}. Message: {1}",
                            param.lms_user_id, rr.Message));
                }

                ProcessDynamicProvisioning(provider, lmsLicense, currentMeeting, lmsUser, lmsUserDto);
            }

            var loginResult = ACLogin(lmsLicense, param, lmsUser, provider);

            // NOTE: ??? re-apply deleted from AC host role??
            if (currentMeeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                var isOwner = currentMeeting.OfficeHours.LmsUser.UserId.Equals(param.lms_user_id);

                provider.UpdateScoPermissionForPrincipal(
                    currentMeetingScoId,
                    loginResult.User.PrincipalId,
                    isOwner ? MeetingPermissionId.host : MeetingPermissionId.view);
                if (isOwner)
                {
                    this.UsersSetup.AddUsersToMeetingHostsGroup(provider, new[] { loginResult.User.PrincipalId });
                }
            }

            ProcessGuestAuditUsers(provider, lmsLicense, currentMeetingScoId, loginResult.User.PrincipalId, param,
                () => LmsCourseMeetingModel.GetByCompanyAndScoId(lmsLicense, currentMeetingScoId, 0));

            string breezeToken = loginResult.BreezeSession;

            string wstoken = null;

            if (lmsLicense.LmsProviderId == (int)LmsProviderEnum.Blackboard)
            {
                var lmsUserService = LmsFactory.GetUserService(LmsProviderEnum.Blackboard);
                var currentUserRes = await lmsUserService.GetUser(lmsLicense.GetLMSSettings(Settings),
                    param.lms_user_id,
                    param.course_id.ToString(),
                    param);
                var currentUser = currentUserRes.Data;
                //string error = currentUserRes.error;

                if (currentUser != null)
                {
                    wstoken = currentUser.Id;
                }
            }

            SaveLMSUserParameters(param, lmsLicense, loginResult.User.PrincipalId, wstoken);

            var breezeSession = breezeToken ?? string.Empty;
            bool isTeacher = this.UsersSetup.IsTeacher(param, lmsLicense);
            bool forcedAddInInstallation = lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.ForcedAddInInstallation);

            string meetingJoinUrl;
            if (lmsLicense.LoginUsingCookie.GetValueOrDefault())
            {
                meetingJoinUrl = string.Concat(
                    meetingUrl,
                    isTeacher && forcedAddInInstallation ? "?lightning=true" : string.Empty);
            }
            else
            {
                meetingJoinUrl = string.Concat(
                    meetingUrl,
                    isTeacher && forcedAddInInstallation ? "?lightning=true&" : "?",
                    "session=" + breezeToken);
            }
            return (meetingJoinUrl, breezeSession);
        }

        public LoginResult ACLogin(ILmsLicense lmsLicense, LtiParamDTO param, LmsUser lmsUser, IAdobeConnectProxy adminProvider)
        {
            if (lmsUser.PrincipalId == null)
                throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");

            var principalInfo = adminProvider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo;
            Principal registeredUser = principalInfo?.Principal;
            if (registeredUser == null)
            {
                var message = string.Format(
                        Resources.Messages.NoUserByPrincipalIdFound, lmsUser.PrincipalId ?? string.Empty);
                Logger.Error(message);
                throw new Core.WarningMessageException(message);
            }

            string breezeToken = null;
            string generatedPassword = null;
            if (lmsUser.AcConnectionMode == AcConnectionMode.Overwrite && string.IsNullOrEmpty(lmsUser.ACPassword))
            {
                if (lmsLicense.AcUsername.Equals(registeredUser.Login, StringComparison.OrdinalIgnoreCase))
                {
                    generatedPassword = lmsLicense.AcPassword;
                    UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                }
                else
                { 
                    generatedPassword = Password.CreateAlphaNumericRandomPassword(8);
                    var resetPasswordResult = adminProvider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
                    if (resetPasswordResult.Success)
                    {
                        UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                    }
                }
            }
            var password = lmsUser.ACPassword;
            if (!string.IsNullOrEmpty(password))
            {
                breezeToken = AcAccountService.LoginIntoAC(
                    lmsLicense,
                    param,
                    registeredUser,
                    password,
                    adminProvider);
                if (breezeToken == null && generatedPassword == null && lmsUser.AcConnectionMode == AcConnectionMode.Overwrite)
                {
                    // trying to login with new generated password (in case, for example, when user changed his AC password manually)
                    generatedPassword = Membership.GeneratePassword(8, 2);
                    var resetPasswordResult = adminProvider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
                    if (resetPasswordResult.Success)
                    {
                        UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                    }
                    breezeToken = AcAccountService.LoginIntoAC(
                        lmsLicense,
                        param,
                        registeredUser,
                        generatedPassword,
                        adminProvider);
                }
            }

            return new LoginResult { BreezeSession = breezeToken, User = registeredUser };
        }

        public OperationResult LeaveMeeting(ILmsLicense lmsLicense, LtiParamDTO param, int id, IAdobeConnectProxy provider)
        {
            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsLicense.Id, param.course_id, id);

            if (currentMeeting == null)
            {
                return OperationResult.Error(Resources.Messages.MeetingNotFound);
            }

            if (currentMeeting.LmsMeetingType != (int)LmsMeetingType.StudyGroup)
            {
                return OperationResult.Error(Resources.Messages.MeetingNotStudyGroup);
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();

            string login = param.lms_user_login;
            string email = param.lis_person_contact_email_primary;
            
            Principal registeredUser = AcUserService.GetPrincipalByLoginOrEmail(provider, login, email, lmsLicense.ACUsesEmailAsLogin.GetValueOrDefault());

            if (registeredUser != null)
            {
                StatusInfo result = provider.UpdateScoPermissionForPrincipal(
                    currentMeetingScoId,
                    registeredUser.PrincipalId,
                    MeetingPermissionId.remove);

                return result.Code == StatusCodes.ok 
                    ? OperationResult.Success()
                    : OperationResult.Error(string.Format("AC:UpdateScoPermissionForPrincipal error. Code:{0}. SubCode:{1}.", result.Code.ToString(), result.SubCode.ToString()));
            }

            return OperationResult.Error(string.Format(Resources.Messages.MeetingCantFindPrincipal, email, login));
        }
        
        public async Task<OperationResult> SaveMeeting(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            ILtiParam param,
            MeetingDTOInput meetingDTO,
            StringBuilder trace,
            IFolderBuilder fb,
            bool retrieveLmsUsers = false)
        {
            if (lmsLicense == null)
                throw new ArgumentNullException(nameof(lmsLicense));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (meetingDTO == null)
                throw new ArgumentNullException(nameof(meetingDTO));
            if (fb == null)
                throw new ArgumentNullException(nameof(fb));
            
            if ((meetingDTO.GetMeetingType() == LmsMeetingType.StudyGroup) 
                && !UsersSetup.IsTeacher(param, lmsLicense)
                && !lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.CanStudentCreateStudyGroup, true))
            {
                return OperationResult.Error("Students are not allowed to create Study Groups.");
            }

            var lmsUsers = new List<LmsUserDTO>();
            string message = string.Empty;
            LmsCourseMeeting meeting = null;
            var sw = Stopwatch.StartNew();
            
            FixDateTimeFields(meetingDTO);

            LmsUser lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsLicense.Id).Value;
            if (lmsUser == null)
            {
                return OperationResult.Error(string.Format("No lms user found with id={0} and companyLmsId={1}", param.lms_user_id, lmsLicense.Id));
            }

            if (meetingDTO.Id == 0 && meetingDTO.GetMeetingType() == LmsMeetingType.OfficeHours)
            {
                var meetings = LmsCourseMeetingModel.GetAllByCourseId(lmsLicense.Id, param.course_id);
                var ohMeeting =
                    meetings.FirstOrDefault(x => x.OfficeHours != null && x.OfficeHours.LmsUser.Id == lmsUser.Id);
                if (ohMeeting != null)
                {
                    meeting = ohMeeting;
                    message = "There was already created Office Hours meeting";
                }
            }

            LmsCalendarEventDTO lmsCalendarEvent = null;

            if (meeting == null)
            {
                meeting = this.GetCourseMeeting(lmsLicense, param.course_id, meetingDTO.Id ?? 0,
                    meetingDTO.GetMeetingType());
                string meetingSco = meeting.GetMeetingScoId();

                sw.Stop();
                trace.AppendFormat("SaveMeeting: GetOneByUserIdAndCompanyLms+GetCourseMeeting: time: {0}.",
                    sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                OfficeHours officeHours = null;
                if (meetingDTO.GetMeetingType() == LmsMeetingType.OfficeHours)
                {
                    officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                    if (string.IsNullOrEmpty(meetingSco) && (officeHours != null))
                    {
                        meetingSco = officeHours.ScoId;
                        meeting.ScoId = officeHours.ScoId;
                        //meetingDTO.id = meetingSco;
                    }
                }

                bool isNewMeeting = string.IsNullOrEmpty(meetingSco) || !provider.GetScoInfo(meetingSco).Success;

                var updateItem = new MeetingUpdateItem {ScoId = isNewMeeting ? null : meetingSco};

                var currentUserPrincipal = AcUserService.GetPrincipalByLoginOrEmail(
                    provider,
                    param.lms_user_login,
                    param.lis_person_contact_email_primary,
                    lmsLicense.ACUsesEmailAsLogin.GetValueOrDefault());

                sw.Stop();
                trace.AppendFormat("SaveMeeting: GetScoInfo+GetPrincipalByLoginOrEmail: time: {0}.",
                    sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();
                var hostGroup = MeetingTypeFactory.HostGroup(meetingDTO.GetMeetingType());
                //===========================================
                if (isNewMeeting)
                {
                    //NOTE: need to call before use GetMeetingFolder method;
                    // when we call group-membership-update api action ac create folder in the user-meetings directory called as user login;
                    this.UsersSetup.AddUsersToMeetingHostsGroup(provider, new[] { currentUserPrincipal.PrincipalId }, hostGroup);

                    sw.Stop();
                    trace.AppendFormat("SaveMeeting: AddUserToMeetingHostsGroup: time: {0}.", sw.Elapsed.ToString());
                    sw = Stopwatch.StartNew();

                    // NOTE: for meeting we need users to add to AC meeting;
                    // For StudyGroup - to be sure we can get them on 2nd tab (and reuse them if retrieveLmsUsers==true)
                    if ((meeting.LmsMeetingType == (int)LmsMeetingType.Meeting)
                        || (meeting.LmsMeetingType == (int)LmsMeetingType.VirtualClassroom)
                        || (meeting.LmsMeetingType == (int) LmsMeetingType.StudyGroup)
                        || (meeting.LmsMeetingType == (int) LmsMeetingType.Seminar))
                    {
                        var lmsUsersRes = await UsersSetup.GetLMSUsers(lmsLicense,
                            meeting,
                            meeting.CourseId,
                            param);
                        lmsUsers = lmsUsersRes.Item1;
                        var error = lmsUsersRes.Item2;
                        if (error != null)
                        {
                            Logger.Error(error);
                            return OperationResult.Error(Resources.Messages.CantRetrieveLmsUsers);
                        }

                        if (lmsLicense.UseSynchronizedUsers && lmsUsers.Count > Core.Utils.Constants.SyncUsersCountLimit)
                        {
                            meeting.EnableDynamicProvisioning = true;
                        }
                    }

                    sw.Stop();
                    trace.AppendFormat("SaveMeeting: UsersSetup.GetLMSUsers: time: {0}.", sw.Elapsed.ToString());
                    sw = Stopwatch.StartNew();
                }
                //===========================================

                string meetingFolder = fb.GetMeetingFolder(currentUserPrincipal);

                sw.Stop();
                trace.AppendFormat("SaveMeeting: GetMeetingFolder: time: {0}.", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                SetMeetingUpdateItemFields(
                    lmsLicense,
                    meetingDTO,
                    updateItem,
                    meetingFolder,
                    isNewMeeting);

                ProcessMeetingName(lmsLicense, param, meetingDTO, lmsUser,
                    isNewMeeting, updateItem, meeting, officeHours);

                sw.Stop();
                trace.AppendFormat("SaveMeeting: ProcessMeetingName: time: {0}.", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                ScoInfoResult result = isNewMeeting ? provider.CreateSco(updateItem) : provider.UpdateSco(updateItem);

                sw.Stop();
                trace.AppendFormat("SaveMeeting: CreateSco: time: {0}.", sw.Elapsed.ToString());

                if (!result.Success || result.ScoInfo == null)
                {
                    if (!result.Success)
                        Logger.Error($"[CreateSco\\UpdateSco error]: { result.Status.GetErrorInfo() }");

                    if ((result.Status.SubCode == StatusSubCodes.duplicate) && (result.Status.InvalidField == "name"))
                        return OperationResult.Error(Resources.Messages.NotUniqueMeetingName);

                    if ((result.Status.SubCode == StatusSubCodes.duplicate) &&
                        (result.Status.InvalidField == "url-path"))
                        return OperationResult.Error(Resources.Messages.MeetingNotUniqueUrlPath);

                    if ((result.Status.SubCode == StatusSubCodes.denied))
                        return OperationResult.Error(Resources.Messages.AdobeConnectDeniedErrorMessage);

                    return OperationResult.Error(result.Status.Code.ToString() + " " + result.Status.SubCode.ToString());
                }

                //--------------------------- CalendarEvent
                lmsCalendarEvent = await CreateOrUpdateCalendarEvent(lmsLicense, meeting, param, isNewMeeting, result);
                if (isNewMeeting)
                {
                    meeting.LmsCalendarEventId = lmsCalendarEvent?.Id;
                }
                //-----------------------------


                if (meeting.LmsMeetingType == (int)LmsMeetingType.VirtualClassroom)
                {
                    // NOTE: ACLTI-1071 Cannot login to VC's from LTI (TMF-Group) - set it to false for all
                    // NOTE: ignore result
                    // NOTE: ACLTI-2070 Introduced option with default value set to 'false' but when saving license, default value is 'true'
                    var namedVirtualClassroomManager = lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.NamedVirtualClassroomManager, false);
                    provider.UpdateVirtualClassroomLicenseModel(result.ScoInfo.ScoId, namedVirtualClassroomManager);
                }

                bool audioProfileProccesed = ProcessAudio(lmsLicense, param, isNewMeeting, meetingDTO, updateItem.Name,
                    lmsUser, meeting, result.ScoInfo, provider);
                if (!audioProfileProccesed)
                {
                    message += "Meeting was created without audio profile. ";
                }

                if (isNewMeeting)
                {
                    // newly created meeting
                    if (meeting.LmsMeetingType != (int) LmsMeetingType.OfficeHours)
                    {
                        meeting.ScoId = result.ScoInfo.ScoId;
                    }

                    // NOTE: always add current user as host - then process others (for meeting)
                    // two extra calls for meeting type - but more secure (we always will have a meeting's host!)
                    provider.UpdateScoPermissionForPrincipal(
                        result.ScoInfo.ScoId,
                        currentUserPrincipal.PrincipalId,
                        MeetingPermissionId.host);
                }

                bool attachToExistedOfficeHours = false;
                if (meeting.LmsMeetingType == (int) LmsMeetingType.OfficeHours)
                {
                    officeHours = officeHours ?? new OfficeHours {LmsUser = lmsUser};
                    officeHours.Hours = meetingDTO.OfficeHours;
                    officeHours.ScoId = meeting.ScoId = result.ScoInfo.ScoId;
                    this.OfficeHoursModel.RegisterSave(officeHours);

                    meeting.OfficeHours = officeHours;
                    meeting.ScoId = null;
                    attachToExistedOfficeHours = !isNewMeeting && (meeting.Id == 0); // we attach existed office hours meeting for another course
                }
                else if (meeting.LmsMeetingType == (int) LmsMeetingType.StudyGroup)
                {
                    meeting.Owner = lmsUser;
                }
                
                this.LmsCourseMeetingModel.RegisterSave(meeting);
                this.LmsCourseMeetingModel.Flush();

                SpecialPermissionId specialPermissionId = meetingDTO.GetPermissionId();
                provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);

                if (isNewMeeting && ((meeting.LmsMeetingType == (int)LmsMeetingType.Meeting) || (meeting.LmsMeetingType == (int)LmsMeetingType.VirtualClassroom) || (meeting.LmsMeetingType == (int)LmsMeetingType.Seminar))
                    && lmsUsers.Count <= Core.Utils.Constants.SyncUsersCountLimit)
                {
                    if (lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.UseCourseSections))
                    {
                        LmsCourseSectionsServiceBase sectionsService = LmsFactory.GetCourseSectionsService(lmsLicense.LmsProviderId, lmsLicense.GetLMSSettings(Settings), param);
                        var sections = await sectionsService.GetCourseSections();
                        var firstSection = sections.OrderBy(x => x.Id).FirstOrDefault(); //
                        if (firstSection != null)
                        {
                            meeting.CourseSections.Add(new LmsCourseSection
                            {
                                LmsId = firstSection.Id,
                                Name = firstSection.Name,
                                Meeting = meeting
                            });

                            lmsUsers =
                                lmsUsers.Where(
                                    x => x.SectionIds == null || x.SectionIds.Any(s => s == firstSection.Id.ToString())).ToList();
                        }
                    }
                    string msg;
                    List<LmsUserDTO> usersToAddToMeeting = this.UsersSetup.GetUsersToAddToMeeting(lmsLicense, lmsUsers,
                        out msg);
                    message += msg;

                    sw = Stopwatch.StartNew();

                    this.UsersSetup.SetDefaultUsers(
                        lmsLicense,
                        meeting,
                        provider,
                        param.lms_user_id,
                        meeting.CourseId,
                        result.ScoInfo.ScoId,
                        usersToAddToMeeting,
                        param);

                    sw.Stop();
                    trace.AppendFormat("SaveMeeting: UsersSetup.SetDefaultUsers: time: {0}.", sw.Elapsed.ToString());
                }

                if (isNewMeeting || attachToExistedOfficeHours)
                {
                    try
                    {
                        await CreateAnnouncement(
                            (LmsMeetingType) meeting.LmsMeetingType,
                            lmsLicense,
                            param,
                            meetingDTO);
                    }
                    catch (Exception)
                    {
                        message += "Meeting was created without announcement. Please contact administrator. ";
                    }
                }
            }
            sw = Stopwatch.StartNew();

            //TimeZoneInfo timeZone = AcAccountService.GetAccountDetails(provider, IoC.Resolve<ICache>()).TimeZoneInfo;

            //MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
            //    provider,
            //    lmsUser,
            //    param,
            //    lmsLicense,
            //    result.ScoInfo,
            //    meeting,
            //    timeZone);


            // TODO: optimize - we have result.ScoInfo already!!
            MeetingInfo info = this.GetAcMeetingInfo(
                           provider,
                           new List<string> { "public-access", lmsUser.PrincipalId },
                           lmsLicense.Id,
                           meeting.GetMeetingScoId(),
                           meeting,
                           null);
            MeetingDTO updatedMeeting = await BuildDto(
                 lmsUser,
                 param,
                 lmsLicense,
                 info,
                 lmsCalendarEvent,
                 //timeZone,
                 trace);

            if (retrieveLmsUsers)
            {
                var users = await UsersSetup.GetUsers(lmsLicense,
                    provider,
                    param.course_id,
                    param,
                    updatedMeeting.Id ?? 0,
                    lmsUsers);

                sw.Stop();
                trace.AppendFormat("SaveMeeting: GetMeetingDTOByScoInfo+GetUsers: time: {0}.", sw.Elapsed.ToString());
                //sw = Stopwatch.StartNew();

                if (users.Item2 != null)
                {
                    return OperationResult.Error(Resources.Messages.CantRetrieveLmsUsers);
                }

		        return OperationResultWithData<MeetingAndLmsUsersDTO>.Success(message,
                    new MeetingAndLmsUsersDTO
                    {
                        Meeting = updatedMeeting,
                        LmsUsers = users.Item1,
                    });
	        }

            return OperationResultWithData<MeetingDTO>.Success(message, updatedMeeting);
        }

        private async Task<LmsCalendarEventDTO> CreateOrUpdateCalendarEvent(ILmsLicense lmsLicense, LmsCourseMeeting meeting, ILtiParam param, bool isNewMeeting, ScoInfoResult result)
        {
            LmsCalendarEventDTO lmsCalendarEvent = null;

            var lmsCalendarService =
                LmsFactory.GetCalendarEventService((LmsProviderEnum)lmsLicense.LmsProviderId, lmsLicense);

            if (lmsCalendarService == null)
                return null;

            if (meeting.LmsMeetingType != (int) LmsMeetingType.Meeting &&
                meeting.LmsMeetingType != (int) LmsMeetingType.VirtualClassroom &&
                meeting.LmsMeetingType != (int) LmsMeetingType.Seminar)
                return null;

            var lmsSettings = lmsLicense.GetLMSSettings(Settings);

            if (isNewMeeting)
            {
                var eventDto = new LmsCalendarEventDTO
                {
                    StartAt = result.ScoInfo.BeginDate,
                    EndAt = result.ScoInfo.EndDate,
                    Title = result.ScoInfo.Name
                };
                lmsCalendarEvent =
                    await lmsCalendarService.CreateEvent(param.course_id.ToString(), lmsSettings, eventDto);
            }
            else
            {
                if (string.IsNullOrEmpty(meeting.LmsCalendarEventId))
                    return null;

                var eventDto = new LmsCalendarEventDTO
                {
                    Id = meeting.LmsCalendarEventId,
                    StartAt = result.ScoInfo.BeginDate,
                    EndAt = result.ScoInfo.EndDate,
                    Title = result.ScoInfo.Name
                };
                lmsCalendarEvent =
                    await lmsCalendarService.UpdateEvent(param.course_id.ToString(), lmsSettings, eventDto);
            }

            return lmsCalendarEvent;
        }

        private async Task RemoveLmsCalendarEventForMeeting(ILmsLicense lmsLicense, LmsCourseMeeting meeting)
        {
            var lmsCalendarService = LmsFactory.GetCalendarEventService((LmsProviderEnum) lmsLicense.LmsProviderId, lmsLicense);
            if (lmsCalendarService == null)
                return;

            if (!((meeting.LmsMeetingType == (int) LmsMeetingType.Meeting)
                  || (meeting.LmsMeetingType == (int) LmsMeetingType.VirtualClassroom)
                  || (meeting.LmsMeetingType == (int) LmsMeetingType.Seminar)))
                return;

            var lmsSettings = lmsLicense.GetLMSSettings(Settings);
            try
            {
                if (!string.IsNullOrEmpty(meeting.LmsCalendarEventId))
                {
                    await lmsCalendarService.DeleteCalendarEvent(meeting.LmsCalendarEventId, lmsSettings);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        // TODO: move MeetingReuseDTO
        public async Task<OperationResult> ReuseExistedAdobeConnectMeeting(ILmsLicense lmsLicense,
            LmsUser lmsUser,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            MeetingReuseDTO dto,
            bool retrieveLmsUsers)
        {
            ScoInfoResult meetingSco = provider.GetScoInfo(dto.ScoId);
            if (!meetingSco.Success)
            {
                Logger.ErrorFormat("[ReuseExistedAdobeConnectMeeting] Meeting not found in Adobe Connect. {0}.", meetingSco.Status.GetErrorInfo());
                return OperationResult.Error(Resources.Messages.MeetingNotFoundInAC);
            }

            LmsCourseMeeting originalMeeting = this.LmsCourseMeetingModel.GetLtiCreatedByCompanyAndScoId(lmsLicense, dto.ScoId);
            int? sourceLtiCreatedMeetingId = originalMeeting?.Id;
            
            var json = JsonSerializer.JsonSerialize(new MeetingNameInfo
            {
                reusedMeetingName = meetingSco.ScoInfo.Name,
            });
            
            var meeting = new LmsCourseMeeting
            {
                LmsCompanyId = lmsLicense.Id,
                CourseId = param.course_id,
                LmsMeetingType = (int)dto.GetMeetingType(),
                ScoId = dto.ScoId,
                Reused = true,
                SourceCourseMeetingId = sourceLtiCreatedMeetingId,
                MeetingNameJson = json,
            };

            var lmsUsersRes = await UsersSetup.GetLMSUsers(lmsLicense,
                    meeting,
                    meeting.CourseId,
                    param);
            // TRICK: always read users from LMS - we need to sync participant list
            List<LmsUserDTO>  lmsUsers = lmsUsersRes.Item1;
            var error = lmsUsersRes.Item2;
            if (error != null)
            {
                return OperationResult.Error(Resources.Messages.CantRetrieveLmsUsers);
            }

            if (originalMeeting != null)
            {
                MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(originalMeeting.MeetingNameJson)
                    ? new MeetingNameInfo()
                    : JsonDeserializer.JsonDeserialize<MeetingNameInfo>(originalMeeting.MeetingNameJson);

                nameInfo.reusedMeetingName = meetingSco.ScoInfo.Name;
                originalMeeting.MeetingNameJson = JsonSerializer.JsonSerialize(nameInfo);
                LmsCourseMeetingModel.RegisterSave(originalMeeting);
            }
            LmsCourseMeetingModel.RegisterSave(meeting);
            LmsCourseMeetingModel.Flush();

            if (!dto.MergeUsers)
            {
                // NOTE: Clean all existed AC meeting participants.
                MeetingPermissionCollectionResult allMeetingEnrollments = provider.GetAllMeetingEnrollments(dto.ScoId);

                foreach (var chunk in allMeetingEnrollments.Values.Select(
                        enrollment =>
                            new MeetingPermissionUpdateTrio
                            {
                                ScoId = dto.ScoId,
                                PrincipalId = enrollment.PrincipalId,
                                PermissionId = MeetingPermissionId.remove,
                            }).Chunk(provider.GetPermissionChunk()))
                {
                    var status = provider.UpdateScoPermissions(chunk);
                    if (status.Code != StatusCodes.ok)
                    {
                        string errorMsg = string.Format("ReuseExistedAdobeConnectMeeting > UpdateScoPermissionForPrincipal. Status.Code:{0}, Status.SubCode:{1}.",
                            status.Code.ToString(),
                            status.SubCode
                            );
                        throw new InvalidOperationException(errorMsg);
                    }

                }
            }

            if (lmsUsers.Count <= EdugameCloud.Lti.Core.Utils.Constants.SyncUsersCountLimit)
            {
                if (lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.UseCourseSections))
                {
                    LmsCourseSectionsServiceBase sectionsService = LmsFactory.GetCourseSectionsService(lmsLicense.LmsProviderId, lmsLicense.GetLMSSettings(Settings), param);
                    var sections = await sectionsService.GetCourseSections();
                    if (originalMeeting != null)
                    {
                        foreach (var cs in originalMeeting.CourseSections.Where(x => sections.Any(s => s.Id == x.LmsId)))
                        {
                            meeting.CourseSections.Add(cs);
                        }
                    }
                    else
                    {
                        var firstSection = sections.OrderBy(x => x.Id).FirstOrDefault(); //
                        if (firstSection != null)
                        {
                            meeting.CourseSections.Add(new LmsCourseSection
                            {
                                LmsId = firstSection.Id,
                                Name = firstSection.Name,
                                Meeting = meeting
                            });
                        }
                    }

                    lmsUsers =
                        lmsUsers.Where(
                                x =>
                                    x.SectionIds == null ||
                                    x.SectionIds.Any(s => meeting.CourseSections.Any(cs => s == cs.LmsId)))
                            .ToList();
                }

                UsersSetup.SetDefaultUsers(
                    lmsLicense,
                    meeting,
                    provider,
                    param.lms_user_id,
                    meeting.CourseId,
                    meeting.ScoId,
                    lmsUsers,
                    param);
            }

            //TimeZoneInfo timeZone = AcAccountService.GetAccountDetails(provider, IoC.Resolve<ICache>()).TimeZoneInfo;
            //MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
            //    provider,
            //    lmsUser,
            //    param,
            //    credentials,
            //    meetingSco.ScoInfo,
            //    meeting,
            //    timeZone);

            // TODO: optimize - we have meetingSco.ScoInfo already!!
            MeetingInfo info = this.GetAcMeetingInfo(
                           provider,
                           new List<string> { "public-access", lmsUser.PrincipalId },
                           lmsLicense.Id,
                           meeting.GetMeetingScoId(),
                           meeting,
                           null);
            MeetingDTO updatedMeeting = await BuildDto(
                 lmsUser,
                 param,
                 lmsLicense,
                 info,
                 //timeZone,
                 null);

            CreateAnnouncement(
                    (LmsMeetingType)meeting.LmsMeetingType,
                    lmsLicense,
                    param,
                    updatedMeeting,
                    info.Sco.BeginDate);

            if (retrieveLmsUsers)
            {
                var usersRes = await UsersSetup.GetUsers(lmsLicense,
                    provider,
                    param.course_id,
                    param,
                    updatedMeeting.Id ?? 0,
                    lmsUsers);

                if (usersRes.Item2 != null)
                {
                    return OperationResult.Error(Resources.Messages.CantRetrieveLmsUsers);
                }

                return new MeetingAndLmsUsersDTO
                    {
                        Meeting = updatedMeeting,
                        LmsUsers = usersRes.Item1,
                    }.ToSuccessResult();
            }

            return updatedMeeting.ToSuccessResult();
        }

        public async Task<(List<string> data, string error)> DeleteMeetingAsync(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id)
        {
            var model = this.LmsCourseMeetingModel;
            LmsCourseMeeting meeting = model.GetOneByCourseAndId(lmsLicense.Id, param.course_id, id);

            if (meeting == null)
            {
                return (data: new List<string>(), error: Resources.Messages.MeetingNotFound);
            }

            // TRICK: before deletion

            // TODO: REVIEW

            bool acMeetingIsStillUsed = lmsLicense.EnableMeetingReuse
                ? this.LmsCourseMeetingModel.ContainsByCompanyAndScoId(lmsLicense, meeting.GetMeetingScoId(), meeting.Id)
                : false;

            List<MeetingPermissionInfo> enrollments = this.UsersSetup.GetMeetingAttendees(provider, meeting.GetMeetingScoId());

            var meetingSessionService =
                LmsFactory.GetMeetingSessionService(lmsLicense, param);

            await meetingSessionService.DeleteMeetingSessionsAsync(meeting, param);

            model.RegisterDelete(meeting, true);

            if (lmsLicense.EnableMeetingReuse && !acMeetingIsStillUsed)
            {
                if (!meeting.Reused.GetValueOrDefault() || (meeting.Reused.GetValueOrDefault() && meeting.SourceCourseMeetingId.HasValue))
                {
                    provider.DeleteSco(meeting.GetMeetingScoId());
                    DeleteAudioProfile(lmsLicense, meeting, provider);
                }
            }
            else
            {
                provider.DeleteSco(meeting.GetMeetingScoId());
                DeleteAudioProfile(lmsLicense, meeting, provider);
            }

            return (data: enrollments.Select(x => x.Login).ToList(), error: null);
        }
        
        //public void SetupFolders(LmsCompany credentials, IAdobeConnectProxy provider)
        //{
        //    string templatesSco = null;
        //    if (!string.IsNullOrWhiteSpace(credentials.ACTemplateScoId))
        //    {
        //        ScoInfoResult templatesFolder = provider.GetScoInfo(credentials.ACTemplateScoId);
        //        if (templatesFolder.Success && templatesFolder.ScoInfo != null)
        //        {
        //            templatesSco = templatesFolder.ScoInfo.ScoId;
        //        }
        //    }

        //    if (templatesSco == null)
        //    {
        //        ScoContentCollectionResult sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
        //        if (sharedTemplates.ScoId != null)
        //        {
        //            credentials.ACTemplateScoId = sharedTemplates.ScoId;
        //            this.LmsСompanyModel.RegisterSave(credentials);
        //            this.LmsСompanyModel.Flush();
        //        }
        //    }
        //}
        
        //public string GetMeetingFolder(LmsCompany lmsLicense, IAdobeConnectProxy provider, Principal user, bool useLmsUserEmailForSearch)
        //{
        //    string adobeConnectScoId = null;

        //    if (lmsLicense.UseUserFolder.GetValueOrDefault() && user != null)
        //    {
        //        ////TODO Think about user folders + renaming directory
        //        adobeConnectScoId = SetupUserMeetingsFolder(lmsLicense, provider, user, useLmsUserEmailForSearch);
        //    }

        //    if (adobeConnectScoId == null)
        //    {
        //        LmsProvider lmsProvider = LmsProviderModel.GetById(lmsLicense.LmsProviderId);
        //        SetupSharedMeetingsFolder(lmsLicense, lmsProvider, provider);
        //        this.LmsСompanyModel.RegisterSave(lmsLicense);
        //        this.LmsСompanyModel.Flush();
        //        adobeConnectScoId = lmsLicense.ACScoId;
        //    }

        //    return adobeConnectScoId;
        //}
        
        /// <summary>
        /// The get LMS parameters.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="acDomain">
        /// The AC domain.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public LmsUserParametersDTO GetLmsParameters(string acId, string acDomain, string scoId, ref string error)
        {
            LmsCourseMeetingModel.Flush();
            var courseMeetings = LmsCourseMeetingModel.GetAllByMeetingId(scoId);

            var serverCourseMeetings = courseMeetings.Where(
                cm =>
                {
                    var license = LmsСompanyModel.GetOneById(cm.LmsCompanyId).Value;
                    var acServer = license.AcServer;
                    if (string.IsNullOrWhiteSpace(acServer))
                    {
                        return false;
                    }

                    return acDomain.StartsWith(acServer.Trim('/'), StringComparison.InvariantCultureIgnoreCase);
                })
                .ToList();

            if (!serverCourseMeetings.Any())
            {
                error =  Resources.Messages.MeetingNotAssociatedToCourse;
                return null;
            }

            var paramList = new List<LmsUserParameters>();

            foreach (var courseMeeting in serverCourseMeetings)
            {
                var param = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(acId, courseMeeting.CourseId, courseMeeting.LmsCompanyId).Value;
                if (param != null)
                {
                    paramList.Add(param);
                }
            }

            if (!paramList.Any())
                return null;

            var userParameter = paramList.OrderByDescending(p => p.LastLoggedIn).First();

            if (userParameter.CompanyLms.LmsProviderId == (int)LmsProviderEnum.Moodle)
            {
                if (string.IsNullOrEmpty(userParameter.CompanyLms.GetSetting<string>(LmsLicenseSettingNames.MoodleQuizServiceToken))
                    && !string.IsNullOrEmpty(userParameter.CompanyLms.GetSetting<string>(LmsLicenseSettingNames.MoodleCoreServiceToken)))
                {
                    Logger.Warn($"MoodleQuizServiceToken is empty. CompanyLmsId:{userParameter.CompanyLms.Id}");
                    return null;
                }
            }

            LmsProvider lmsProvider = LmsProviderModel.GetById(userParameter.CompanyLms.LmsProviderId);
            return new LmsUserParametersDTO(userParameter, lmsProvider);
        }

        #endregion

        #region Methods
        private void ProcessMeetingName(ILmsLicense lmsLicense, ILtiParam param, MeetingDTO meetingDTO, LmsUser lmsUser,
            bool isNewMeeting, MeetingUpdateItem updateItem, LmsCourseMeeting meeting, OfficeHours officeHours)
        {
            string courseId = meetingDTO.GetMeetingType() == LmsMeetingType.OfficeHours
                ? lmsUser.Id.ToString(CultureInfo.InvariantCulture)
                : param.course_id.ToString(CultureInfo.InvariantCulture);

            int formatterId = lmsLicense.MeetingNameFormatterId;
            IMeetingNameFormatter formatter = MeetingNameFormatterFactory.GetFormatter(formatterId);

            // officeHours - map existing AC meeting but new LmsCourseMeeting record
            if (isNewMeeting)
            {
                string acMeetingName = formatter.BuildName(meetingDTO, param, courseId);
                updateItem.Name = acMeetingName;

                // TODO: move TO formatter base?
                //POPOVR
                var json = JsonSerializer.JsonSerialize(new MeetingNameInfo
                {
                    courseId = courseId,
                    courseNum = param.context_label,
                    courseName = param.context_title,
                    meetingName = meetingDTO.Name,
                    date = DateTime.Today.ToString("MM/dd/yy"),
                });
                meeting.MeetingNameJson = json;
            }
            // TODO: !!!
            // NOTE: use already existed OfficeHours meeting
            else if (!isNewMeeting && (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours) && (meeting.Id == 0))
            {
                string acMeetingName = formatter.BuildName(meetingDTO, param, courseId);
                updateItem.Name = acMeetingName;

                // TODO: move TO formatter base?
                var json = JsonSerializer.JsonSerialize(new MeetingNameInfo
                {
                    courseId = courseId,
                    courseNum = param.context_label,
                    meetingName = meetingDTO.Name,
                    date = DateTime.Today.ToString("MM/dd/yy"),
                });
                meeting.MeetingNameJson = json;
            }
            else
            {
                // HACK: TEST!!!
                // TODO: is it OK to use the setting here? if (lmsLicense.EnableMeetingReuse)
                var thisScoMeetings = lmsLicense.EnableMeetingReuse
                    ? LmsCourseMeetingModel.GetByCompanyAndScoId(lmsLicense, meeting.GetMeetingScoId(), meeting.Id)
                    : Enumerable.Empty<LmsCourseMeeting>();

                // NOTE: it's reused meeting or source for any reused meeting
                //if (!string.IsNullOrEmpty(nameInfo.reusedMeetingName) || meeting.Reused.GetValueOrDefault())
                if (thisScoMeetings.Any())
                {
                    //MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
                    //nameInfo.reusedMeetingName = meetingDTO.name;
                    //meeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);
                    updateItem.Name = meetingDTO.Name;

                    // NOTE: find original LTI meeting and all its reusings; and change their name AS WELL
                    //var thisScoMeetings = LmsCourseMeetingModel.GetByCompanyAndScoId(lmsLicense, meeting.GetMeetingScoId());
                    foreach (LmsCourseMeeting m in thisScoMeetings)
                    {
                        MeetingNameInfo name = JsonDeserializer.JsonDeserialize<MeetingNameInfo>(m.MeetingNameJson);
                        name.reusedMeetingName = meetingDTO.Name;
                        m.MeetingNameJson = JsonSerializer.JsonSerialize(name);
                        LmsCourseMeetingModel.RegisterSave(m);
                    }
                    MeetingNameInfo currentMeetingName = JsonDeserializer.JsonDeserialize<MeetingNameInfo>(meeting.MeetingNameJson);
                    currentMeetingName.reusedMeetingName = meetingDTO.Name;
                    meeting.MeetingNameJson = JsonSerializer.JsonSerialize(currentMeetingName);
                    LmsCourseMeetingModel.RegisterSave(meeting);  // not required for current ?
                }
                else
                {
                    if (meeting.Reused.GetValueOrDefault())
                    {
                        MeetingNameInfo currentMeetingName = JsonDeserializer.JsonDeserialize<MeetingNameInfo>(meeting.MeetingNameJson);
                        currentMeetingName.reusedMeetingName = meetingDTO.Name;
                        meeting.MeetingNameJson = JsonSerializer.JsonSerialize(currentMeetingName);
                    }

                    string acMeetingName = formatter.UpdateName(meeting, meetingDTO.Name);
                    updateItem.Name = acMeetingName;
                }
            }

            if (!isNewMeeting && (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours))
            {
                var coursesWithThisOfficeHours = this.LmsCourseMeetingModel.GetAllByOfficeHoursId(officeHours.Id);
                foreach (LmsCourseMeeting officeHoursMeeting in coursesWithThisOfficeHours.
                    Where(x => !string.IsNullOrWhiteSpace(x.MeetingNameJson)))
                {
                    MeetingNameInfo nameInfo = JsonDeserializer.JsonDeserialize<MeetingNameInfo>(officeHoursMeeting.MeetingNameJson);
                    nameInfo.meetingName = meetingDTO.Name;
                    officeHoursMeeting.MeetingNameJson = JsonSerializer.JsonSerialize(nameInfo);
                    LmsCourseMeetingModel.RegisterSave(officeHoursMeeting);
                }
            }
        }

        private bool ProcessAudio(ILmsLicense lmsLicense, ILtiParam param, bool isNewMeeting, 
            MeetingDTO meetingDTO, string acMeetingName, LmsUser lmsUser,
            LmsCourseMeeting meeting, ScoInfo scoInfo, IAdobeConnectProxy provider)
        {
            // NOTE: do nothing for seminars
            if ((LmsMeetingType)meeting.LmsMeetingType == LmsMeetingType.Seminar)
                return true;

            TelephonyProfileOption option = lmsLicense.GetTelephonyOption((LmsMeetingType)meeting.LmsMeetingType);
            if (option == TelephonyProfileOption.TurnOff)
                return true;

            if (option == TelephonyProfileOption.ReuseExistingProfile)
            {
                // Profile was not selected
                if (string.IsNullOrWhiteSpace(meetingDTO.AudioProfileId))
                {
                    // if it's not meeting creation - remove
                    if (!isNewMeeting)
                    {
                        AudioProfileService.RemoveAudioProfileFromMeeting(scoInfo.ScoId, provider);
                        meeting.AudioProfileId = null;
                        meeting.AudioProfileProvider = null;
                    }

                    return true;
                }

                var audioUpdateResult = AudioProfileService.AddAudioProfileToMeeting(scoInfo.ScoId, meetingDTO.AudioProfileId, provider);
                if (audioUpdateResult.IsSuccess)
                {
                    meeting.AudioProfileId = meetingDTO.AudioProfileId;
                    meeting.AudioProfileProvider = null;
                    return true;
                }                
            }
            else if ((option == TelephonyProfileOption.GenerateNewProfile))
            {
                // if generatenew - we do nothing dirung update.
                if (!isNewMeeting)
                    return true;

                string providerName = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.Telephony.ActiveProfile).ToUpper();

                if (TelephonyDTO.SupportedProfiles.None.Equals(providerName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("GenerateNewProfile option with None provider");

                string profileName = acMeetingName;

                TelephonyProfile profile = IoC.Resolve<ITelephonyProfileEngine>(providerName).CreateProfile(lmsLicense, param, profileName, provider);

                if (profile != null)
                {
                    var audioUpdateResult = AudioProfileService.AddAudioProfileToMeeting(scoInfo.ScoId, profile.ProfileId, provider);
                    if (audioUpdateResult.IsSuccess)
                    {
                        meeting.AudioProfileId = profile.ProfileId;
                        meeting.AudioProfileProvider = providerName;
                        return true;
                    }
                }
            }

            return false;
        }

        private void DeleteAudioProfile(ILmsLicense lmsLicense, LmsCourseMeeting meeting, IAdobeConnectProxy provider)
        {
            if (string.IsNullOrWhiteSpace(meeting.AudioProfileId))
                return;

            // NOTE: do nothing for seminars
            if ((LmsMeetingType)meeting.LmsMeetingType == LmsMeetingType.Seminar)
                return;

            TelephonyProfileInfoResult profile = provider.TelephonyProfileInfo(meeting.AudioProfileId);
            // NOTE: if profile wasn't generated - do not delete it from AC
            if (string.IsNullOrWhiteSpace(meeting.AudioProfileProvider))
                return;

            AudioProfileService.DeleteAudioProfile(meeting.AudioProfileId, provider);

            TelephonyProfileOption option = lmsLicense.GetTelephonyOption((LmsMeetingType)meeting.LmsMeetingType);
            if (option != TelephonyProfileOption.GenerateNewProfile)
                return;

            string providerName = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.Telephony.ActiveProfile).ToUpper();
            // Trying to delete profile from Meeting1 - only if it was generated by M1 integration
            if (meeting.AudioProfileProvider == providerName)
                IoC.Resolve<ITelephonyProfileEngine>(providerName).DeleteProfile(lmsLicense, profile);
        }

        //private Recording GetScheduledRecording(string recordingScoId, string meetingScoId, IAdobeConnectProxy adobeConnectProvider)
        //{
        //    var recordingsByMeeting = adobeConnectProvider.GetRecordingsList(meetingScoId);
        //    if (recordingsByMeeting == null || !recordingsByMeeting.Success || recordingsByMeeting.Values == null || !recordingsByMeeting.Values.Any())
        //    {
        //        return null;
        //    }

        //    return recordingsByMeeting.Values.SingleOrDefault(x => x.ScoId == recordingScoId);
        //}

        private void SaveLMSUserParameters(
            string lmsCourseId,
            LmsCompany lmsLicense,
            string lmsUserId,
            string adobeConnectUserId,
            string courseName,
            string userEmail,
            string userId)
        {
            LmsUserParameters lmsUserParameters = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(adobeConnectUserId, lmsCourseId, lmsLicense.Id).Value;

            if (lmsUserParameters == null)
            {
                lmsUserParameters = new LmsUserParameters
                {
                    AcId = adobeConnectUserId,
                    Course = lmsCourseId,
                    CompanyLms = lmsLicense
                };
            }

            lmsUserParameters.Wstoken = userId;
            lmsUserParameters.LastLoggedIn = DateTime.UtcNow;
            lmsUserParameters.CourseName = courseName;
            lmsUserParameters.UserEmail = userEmail;
            lmsUserParameters.LmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsLicense.Id).Value;
            this.LmsUserParametersModel.RegisterSave(lmsUserParameters);
        }
        
        private bool CanEdit(ILtiParam param, LmsCourseMeeting meeting, ILmsLicense lmsLicense)
        {
            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                return meeting.OfficeHours.LmsUser.UserId.Equals(param.lms_user_id);
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                return meeting.Owner.UserId.Equals(param.lms_user_id);
            }

            return UsersSetup.IsTeacher(param, lmsLicense);
        }
        
        private bool CanJoin(
            LmsUser lmsUser,
            LmsMeetingType meetingType,
            IEnumerable<MeetingPermissionInfo> permission)
        {
            if (meetingType == LmsMeetingType.OfficeHours)
            {
                return true;
            }
            // this method is called after the user has opened the application through LtiController, so there should already be Principal found and saved for the user.
            if (string.IsNullOrWhiteSpace(lmsUser.PrincipalId))
            {
                throw new InvalidOperationException($"lmsUser.PrincipalId is empty. LmsUserID: {lmsUser.Id}");
            }
            
            return (permission != null)
                && permission
                .Where(x => x.PrincipalId == lmsUser.PrincipalId)
                .Select(x => x.PermissionId)
                .Intersect(new List<MeetingPermissionId> { MeetingPermissionId.host, MeetingPermissionId.mini_host, MeetingPermissionId.view })
                .Any();
        }

        private IEnumerable<LmsCompanyRoleMapping> GetGuestAuditRoleMappings(ILmsLicense lmsLicense, ILtiParam param)
        {
            if (!lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.EnableAuditGuestEntry))
                return Enumerable.Empty<LmsCompanyRoleMapping>();
            var customRoles = lmsLicense.RoleMappings.Where(x => !x.IsDefaultLmsRole && new[] { AcRole.Host.Id, AcRole.Presenter.Id }.Contains(x.AcRole));
            var currentUserLtiRoles = new List<string>();
            if (!string.IsNullOrEmpty(param.roles))
            {
                currentUserLtiRoles.AddRange(param.roles.Split(',', ';').Select(x => x.Trim()));
            }

            return customRoles.Where(x => currentUserLtiRoles.Any(lr => lr.Equals(x.LmsRoleName)));
        }

        public void ProcessGuestAuditUsers(IAdobeConnectProxy provider, ILmsLicense lmsLicense, string scoId, 
            string principalId, LtiParamDTO param, Func<IEnumerable<LmsCourseMeeting>> retrieveMeetings)
        {

            var auditRoles = GetGuestAuditRoleMappings(lmsLicense, param);
            if (auditRoles.Any())
            {
                MeetingPermissionCollectionResult meetingEnrollments =
                    provider.GetAllMeetingEnrollments(scoId);

                if (meetingEnrollments.Values.All(x => x.PrincipalId != principalId))
                {
                    AcRole role = auditRoles.Any(x => x.AcRole == AcRole.Host.Id) ? AcRole.Host : AcRole.Presenter;
                    StatusInfo status = provider.UpdateScoPermissionForPrincipal(scoId, principalId, role.MeetingPermissionId);

                    var courseMeetings = retrieveMeetings();
                    foreach (var courseMeeting in courseMeetings)
                    {
                        if ((LmsMeetingType)courseMeeting.LmsMeetingType != LmsMeetingType.StudyGroup) // study groups are usually private meetings => disable automatic addition to meeting
                        {
                            // Add user as guest to DB
                            var guest = new LmsCourseMeetingGuest
                            {
                                PrincipalId = principalId,
                                LmsCourseMeeting = courseMeeting
                            };

                            courseMeeting.MeetingGuests.Add(guest);
                            this.LmsCourseMeetingModel.RegisterSave(courseMeeting, flush: true);
                        }
                    }
                }
            }
        }

        public void ProcessDynamicProvisioning(IAdobeConnectProxy provider, ILmsLicense lmsLicense, LmsCourseMeeting courseMeeting, LmsUser lmsUser, LmsUserDTO lmsUserDto)
        {
            if(lmsUser?.PrincipalId != null && lmsUserDto != null)
            { 
                var acRole = new RoleMappingService().SetAcRole(lmsLicense, lmsUserDto);
                StatusInfo status = provider.UpdateScoPermissionForPrincipal(courseMeeting.GetMeetingScoId(),
                    lmsUser.PrincipalId, acRole);
            }
        }
        

        private MeetingInfo GetAcMeetingInfo(IAdobeConnectProxy provider,
            List<string> principalsToFetchPermissions,
            int lmsCompanyId,
            string meetingScoId,
            LmsCourseMeeting lmsCourseMeeting,
            StringBuilder trace = null)
        {
            var info = new MeetingInfo
            {
                DbRecord =  lmsCourseMeeting,
            };

            var psw = Stopwatch.StartNew();
            ScoInfoResult scoResult = provider.GetScoInfo(meetingScoId);
            psw.Stop();
            trace?.AppendFormat("\t GetMeetings - AC GetScoInfo time: {0}. MeetingSCO-ID: {1}\r\n", psw.Elapsed.ToString(), meetingScoId);

            if (!scoResult.Success || scoResult.ScoInfo == null)
            {
                Logger.WarnFormat("Meeting not found in AC. Meeting sco-id: {0}. CompanyLmsId: {1}.", meetingScoId, lmsCompanyId);
                return info;
            }

            info.Sco = scoResult.ScoInfo;

            psw = Stopwatch.StartNew();
            bool meetingExistsInAC;
            IEnumerable<MeetingPermissionInfo> permission = provider.GetMeetingPermissions(scoResult.ScoInfo.ScoId,
                principalsToFetchPermissions,
                out meetingExistsInAC).Values;

            psw.Stop();
            trace?.AppendFormat("\t GetMeetings - AC GetMeetingPermissions time: {0}. MeetingId: {1}\r\n", psw.Elapsed.ToString(), lmsCourseMeeting.Id);

            info.Permissions = permission;

            if (!string.IsNullOrWhiteSpace(lmsCourseMeeting.AudioProfileId))
            {
                TelephonyProfileInfoResult profile = provider.TelephonyProfileInfo(lmsCourseMeeting.AudioProfileId);
                info.AudioProfile = profile;
                if (profile.TelephonyProfile == null)
                    Logger.Warn($"Audio profile is in DB but not found in AC. ProfileId:{lmsCourseMeeting.AudioProfileId}. MeetingID:{lmsCourseMeeting.Id}");
            }
            return info;
        }

        private async Task<MeetingDTO> BuildDto(
            LmsUser lmsUser,
            ILtiParam param,
            ILmsLicense lmsLicense,
            MeetingInfo meeting,
            LmsCalendarEventDTO lmsCalendarEventDto,
            //TimeZoneInfo timeZone,
            StringBuilder trace = null)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));
            if (lmsUser == null)
                throw new ArgumentNullException(nameof(lmsUser));
            if (lmsLicense == null)
                throw new ArgumentNullException(nameof(lmsLicense));

            if (meeting.Sco == null)
                return null;

            var type = (LmsMeetingType)meeting.DbRecord.LmsMeetingType;
            string officeHoursString = (type == LmsMeetingType.OfficeHours) ? meeting.DbRecord.OfficeHours.Hours : null;

            string meetingName = string.Empty;
            // NOTE: support created meetings; update MeetingNameJson
            if (string.IsNullOrWhiteSpace(meeting.DbRecord.MeetingNameJson))
            {
                int bracketIndex = meeting.Sco.Name.IndexOf("]", StringComparison.Ordinal);
                meetingName =
                    meeting.Sco.Name.Substring(bracketIndex < 0 || (bracketIndex + 2 > meeting.Sco.Name.Length)
                        ? 0
                        : bracketIndex + 2);

                string js = JsonSerializer.JsonSerialize(new MeetingNameInfo
                {
                    courseId = param.course_id.ToString(),
                    courseNum = param.context_label,
                    meetingName = meetingName,
                    date = meeting.Sco.DateCreated.ToString("MM/dd/yy"),
                });

                meeting.DbRecord.MeetingNameJson = js;
                LmsCourseMeetingModel.RegisterSave(meeting.DbRecord, flush: true);
            }
            else
            {
                MeetingNameInfo nameInfo =
                    JsonDeserializer.JsonDeserialize<MeetingNameInfo>(meeting.DbRecord.MeetingNameJson);
                // NOTE: it is reused meeting or source of reusing
                meetingName = string.IsNullOrWhiteSpace(nameInfo.reusedMeetingName) ? nameInfo.meetingName : nameInfo.reusedMeetingName;
            }

            var sw = Stopwatch.StartNew();
            // HACK: TEST!!!
            bool scoIdReused = false;
            int usedByAnotherMeeting = 0;
            if (lmsLicense.EnableMeetingReuse)
            {
                usedByAnotherMeeting = LmsCourseMeetingModel.CourseCountByCompanyAndScoId(lmsLicense,
                    meeting.Sco.ScoId,
                    meeting.DbRecord.Id);
                scoIdReused = meeting.DbRecord.Reused.HasValue && meeting.DbRecord.Reused.Value;
            }

            MeetingSessionDTO[] sessions = null;
            if (lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.EnableMeetingSessions))
            {
                var sessionsService = LmsFactory.GetMeetingSessionService(lmsLicense, param);
                sessions = (await sessionsService.GetSessions(meeting.DbRecord.Id)).ToArray();
                //sessions = meeting.DbRecord.MeetingSessions.Select(x => new MeetingSessionDTO
                //{
                //    Id = x.Id,
                //    Name = x.Name,
                //    StartDate = x.StartDate, //ToString("MM/dd/yyyy hh:mm tt"),
                //    EndDate = x.EndDate, //.ToString("MM/dd/yyyy hh:mm tt"),
                //    Summary = x.Summary,
                //}).ToArray();
            }

            if (usedByAnotherMeeting == 0 && type == LmsMeetingType.OfficeHours)
            {
                var coursesWithThisOfficeHours =
                    this.LmsCourseMeetingModel.GetAllByOfficeHoursId(meeting.DbRecord.OfficeHours.Id);
                usedByAnotherMeeting =
                    coursesWithThisOfficeHours.Where(c => c.Id != meeting.DbRecord.Id)
                        .Select(x => x.CourseId)
                        .Distinct()
                        .Count();
            }
            sw.Stop();
            trace?.AppendFormat("\t GetMeetings - DB GetByCompanyAndScoId time: {0}. MeetingId: {1}\r\n",
                sw.Elapsed.ToString(), meeting.DbRecord.Id);

            var ret = new MeetingDTO
            {
                Id = meeting.DbRecord.Id,
                AcRoomUrl = meeting.Sco.UrlPath.Trim('/'),
                Name = meetingName,
                // NOTE: to skip "" from serialization
                Summary = string.IsNullOrWhiteSpace(meeting.Sco.Description) ? null : meeting.Sco.Description,
                ClassRoomId = string.IsNullOrWhiteSpace(meeting.Sco.ScoTag) ? null : meeting.Sco.ScoTag,
                Template = meeting.Sco.SourceScoId,
                StartTimeStamp =
                    (long) meeting.Sco.BeginDate.ConvertToUnixTimestamp(),
                    //+ (long) GetTimezoneShift(timeZone, meeting.Sco.BeginDate),
                Duration = (meeting.Sco.EndDate - meeting.Sco.BeginDate).ToString(@"h\:mm"),

                //AccessLevel = meeting.GetPublicAccessPermission(),
                //CanJoin = canJoin,
                //IsEditable = isEditable,

                Type = (int) type,
                OfficeHours = officeHoursString,
                Reused = scoIdReused,
                ReusedByAnotherMeeting = usedByAnotherMeeting,

                AudioProfileId = meeting.DbRecord.AudioProfileId, // TODO: use meetingSco.TelephonyProfile
                Sessions = (sessions != null && sessions.Length > 0) ? sessions : null,
                SectionIds = meeting.DbRecord.CourseSections.Any() ? meeting.DbRecord.CourseSections.Select(x => x.LmsId).ToList() : null
            };

            // TRICK: for API
            if (!string.IsNullOrWhiteSpace(lmsUser.PrincipalId))
                SetPermissions(
                    ret,
                    lmsUser,
                    param,
                    lmsLicense,
                    meeting);

            if (meeting.AudioProfile?.TelephonyProfile != null)
            {
                ret.AudioProfileName = meeting.AudioProfile.TelephonyProfile.ProfileName;

                if (lmsLicense.GetTelephonyOption((LmsMeetingType) meeting.DbRecord.LmsMeetingType) ==
                    TelephonyProfileOption.GenerateNewProfile)
                    ret.TelephonyProfileFields = new TelephonyProfileHumanizer().Humanize(meeting.AudioProfile.TelephonyProfileFields);
            }

            return ret;
        }

        private void SetPermissions(
            MeetingDTO dto,
            LmsUser lmsUser,
            ILtiParam param,
            ILmsLicense lmsLicense,
            MeetingInfo meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));
            if (lmsUser == null)
                throw new ArgumentNullException(nameof(lmsUser));
            if (lmsLicense == null)
                throw new ArgumentNullException(nameof(lmsLicense));

            bool isEditable = this.CanEdit(param, meeting.DbRecord, lmsLicense);
            var type = (LmsMeetingType)meeting.DbRecord.LmsMeetingType;

            var canJoin = this.CanJoin(lmsUser, type, meeting.Permissions)
                          || (type != LmsMeetingType.StudyGroup
                              && (GetGuestAuditRoleMappings(lmsLicense, param).Any()
                                  || (lmsLicense.UseSynchronizedUsers && meeting.DbRecord.EnableDynamicProvisioning)));

            dto.AccessLevel = meeting.GetPublicAccessPermission().Replace("-", "_");
            dto.CanJoin = canJoin;
            dto.IsEditable = isEditable;
        }

        //private static double GetTimezoneShift(TimeZoneInfo timezone, DateTime value)
        //{
        //    if (timezone == null)
        //        return 0;

        //    var offset = timezone.GetUtcOffset(value).TotalMilliseconds;
        //    return offset;
        //}
        
        private void SaveLMSUserParameters(LtiParamDTO param, LmsCompany lmsLicense, string adobeConnectUserId, string wstoken)
        {
            this.SaveLMSUserParameters(param.course_id, lmsLicense, param.lms_user_id, adobeConnectUserId, param.context_title, param.lis_person_contact_email_primary, wstoken);
        }

        private static void FixDateTimeFields(MeetingDTOInput meetingDTO)
        {
            if (meetingDTO.StartTime != null)
            {
                meetingDTO.StartTime = meetingDTO.StartTime.PadLeft(8, '0');
            }

            if (meetingDTO.StartDate != null)
            {
                meetingDTO.StartDate = meetingDTO.StartDate.Substring(6, 4) + "-" + meetingDTO.StartDate.Substring(0, 5);
            }
        }

        public LmsCourseMeeting GetCourseMeeting(ILmsLicense lmsLicense, string courseId, long id, LmsMeetingType type)
        {
            LmsCourseMeeting meeting = null;

            if (id > 0)
                meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsLicense.Id, courseId, id);

            // TRICK: we should always pass ID from client if it's UPDATE operation for meeting within the course
            //if (meeting == null && type == LmsMeetingType.OfficeHours)
            //{
            //    meeting =
            //        this.LmsCourseMeetingModel.GetOneByCourseAndType(
            //            lmsLicense.Id,
            //            courseId,
            //            LmsMeetingType.OfficeHours).Value;
            //}

            if (meeting == null)
            {
                meeting = new LmsCourseMeeting
                {
                    LmsCompanyId = lmsLicense.Id,
                    CourseId = courseId,
                    LmsMeetingType = (int)type,
                    // HACK: IMPLEMENT
                    //ScoId = type == LmsMeetingType.OfficeHours ? scoId : null,
                };
            }

            return meeting;
        }

        private void SetMeetingUpdateItemFields(
            ILmsLicense lmsLicense,
            MeetingDTOInput meetingDTO, 
            MeetingUpdateItem updateItem, 
            string folderSco, 
            bool isNew)
        {
            updateItem.Description = meetingDTO.Summary;
            updateItem.FolderId = folderSco;
            updateItem.Language = LanguageModel.GetById(lmsLicense.LanguageId).TwoLetterCode;
            updateItem.Type = ScoType.meeting;
            updateItem.ScoTag = string.IsNullOrWhiteSpace(meetingDTO.ClassRoomId) ? null : meetingDTO.ClassRoomId;
            
            if (isNew)
            {
                updateItem.SourceScoId = meetingDTO.Template;
                updateItem.UrlPath = meetingDTO.AcRoomUrl;
            }

            if (meetingDTO.StartDate == null || meetingDTO.StartTime == null)
            {
                updateItem.DateBegin = DateTime.Now.ToString(AcDateFormat);
                updateItem.DateEnd = DateTime.Now.AddHours(1).ToString(AcDateFormat);
            }

            DateTime dateBegin;

            if (DateTime.TryParse(meetingDTO.StartDate + " " + meetingDTO.StartTime, out dateBegin))
            {
                updateItem.DateBegin = dateBegin.ToString(AcDateFormat);
                TimeSpan duration;
                if (TimeSpan.TryParse(meetingDTO.Duration, out duration))
                {
                    updateItem.DateEnd =
                        dateBegin.AddMinutes((int)duration.TotalMinutes).ToString(AcDateFormat);
                }
            }

            if (meetingDTO.Type == (int)LmsMeetingType.VirtualClassroom)
            {
                updateItem.Icon = "virtual-classroom";
            }
        }

        //private static void SetupSharedMeetingsFolder(LmsCompany credentials, LmsProvider lmsProvider, IAdobeConnectProxy provider)
        //{
        //    string ltiFolderSco = null;
        //    string name = credentials.UserFolderName ?? lmsProvider.LmsProviderName;
        //    name = name.TruncateIfMoreThen(60);
        //    if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
        //    {
        //        ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
        //        if (canvasFolder.Success && canvasFolder.ScoInfo != null)
        //        {
        //            if (canvasFolder.ScoInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        //            {
        //                ltiFolderSco = canvasFolder.ScoInfo.ScoId;
        //            }
        //            else
        //            {
        //                ScoInfoResult updatedSco =
        //                    provider.UpdateSco(
        //                        new FolderUpdateItem
        //                            {
        //                                ScoId = canvasFolder.ScoInfo.ScoId,
        //                                Name = name,
        //                                FolderId = canvasFolder.ScoInfo.FolderId,
        //                                Type = ScoType.folder
        //                            });
        //                if (updatedSco.Success && updatedSco.ScoInfo != null)
        //                {
        //                    ltiFolderSco = updatedSco.ScoInfo.ScoId;
        //                }
        //            }
        //        }
        //    }

        //    if (ltiFolderSco == null)
        //    {
        //        ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
        //        if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
        //        {
        //            ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);
        //            if (existingFolder != null)
        //            {
        //                credentials.ACScoId = existingFolder.ScoId;
        //            }
        //            else
        //            {
        //                ScoInfoResult newFolder = provider.CreateSco(new FolderUpdateItem { Name = name, FolderId = sharedMeetings.ScoId, Type = ScoType.folder });
        //                if (newFolder.Success && newFolder.ScoInfo != null)
        //                {
        //                    provider.UpdatePublicAccessPermissions(newFolder.ScoInfo.ScoId, SpecialPermissionId.denied);
        //                    credentials.ACScoId = newFolder.ScoInfo.ScoId;
        //                }
        //            }
        //        }
        //    }
        //}

        //private static void CreateUserFoldersStructure(string folderScoId, IAdobeConnectProxy provider, 
        //    string userFolderName,
        //    string userMeetingsFolderName,
        //    out string innerFolderScoId)
        //{
        //    var folderContent = provider.GetContentsByScoId(folderScoId);
        //    var userFolder = folderContent.Values.FirstOrDefault(x => x.Name == userFolderName);
        //    if (userFolder == null)
        //    {
        //        var userFolderScoId = CreateFolder(folderScoId, userFolderName, provider);
        //        var userMeetingsFolderScoId = CreateFolder(userFolderScoId, userMeetingsFolderName, provider);
        //        innerFolderScoId = userMeetingsFolderScoId;
        //        return;
        //    }

        //    var userFolderContent = provider.GetContentsByScoId(userFolder.ScoId);
        //    var userMeetingsFolder = userFolderContent.Values.FirstOrDefault(x => x.Name == userMeetingsFolderName);
        //    if (userMeetingsFolder == null)
        //    {
        //        innerFolderScoId =  CreateFolder(userFolder.ScoId, userMeetingsFolderName, provider);
        //        return;
        //    }

        //    innerFolderScoId = userMeetingsFolder.ScoId;
        //}

        //private static string CreateFolder(string folderScoId, string folderName, IAdobeConnectProxy provider)
        //{
        //    var newFolder = provider.CreateSco(new FolderUpdateItem
        //    {
        //        Name = folderName.TruncateIfMoreThen(60),
        //        FolderId = folderScoId,
        //        Type = ScoType.folder
        //    });

        //    if(!newFolder.Success)
        //    {
        //        var msg =string.Format("[AdobeConnectProxy Error] CreateSco " + "Parameters: FolderId:{0}, Name:{1}", folderScoId, folderName);
        //        throw new InvalidOperationException(msg);
        //    }
        //    return newFolder.ScoInfo.ScoId;

        //}
        
        //private static string SetupUserMeetingsFolder(LmsCompany lmsLicense, IAdobeConnectProxy provider,
        //    Principal user, bool useLmsUserEmailForSearch)
        //{
        //    var shortcut = provider.GetShortcutByType("user-meetings");

        //    var userFolderName = useLmsUserEmailForSearch ? user.Email : user.Login;
        //    var meetingsFolderName = string.IsNullOrEmpty(lmsLicense.UserFolderName)
        //        ? userFolderName
        //        : lmsLicense.UserFolderName;
        //    string meetingFolderScoId;

        //    CreateUserFoldersStructure(shortcut.ScoId, provider, userFolderName,
        //        meetingsFolderName, out meetingFolderScoId);
        //    return meetingFolderScoId;
        //}

        #endregion

        public async Task UpdateMeetingCourseSectionsAsync(ILmsLicense lmsLicense, UpdateCourseSectionsDto updateCourseSectionsDto, LtiParamDTO param)
        {
            var meeting = LmsCourseMeetingModel.GetOneById(updateCourseSectionsDto.MeetingId).Value;

            LmsCourseSectionsServiceBase sectionsService = LmsFactory.GetCourseSectionsService(lmsLicense.LmsProviderId, lmsLicense.GetLMSSettings(Settings), param);
            var sections = await sectionsService.GetCourseSections();
            var sectionsToRemove = meeting.CourseSections.Where(x => updateCourseSectionsDto.SectionIds.All(s => s != x.LmsId)).ToList();
            foreach (var lmsCourseSection in sectionsToRemove)
            {
                meeting.CourseSections.Remove(lmsCourseSection);
            }

            var sectionsToAdd = sections.Where(x => updateCourseSectionsDto.SectionIds.Any(s => s == x.Id) && meeting.CourseSections.All(cs => cs.LmsId != x.Id));
            foreach (var lmsCourseSectionDto in sectionsToAdd)
            {
                meeting.CourseSections.Add(new LmsCourseSection
                {
                    LmsId = lmsCourseSectionDto.Id,
                    Name = lmsCourseSectionDto.Name,
                    Meeting = meeting
                });
            }

            if (lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.UseSynchronizedUsers))
            {
                await SynchronizationUserService.SynchronizeUsers(lmsLicense, true, meetingIds: new[] {meeting.Id});
            }
        }
    }

}
