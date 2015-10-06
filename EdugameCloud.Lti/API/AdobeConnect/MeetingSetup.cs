using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using Castle.Core.Logging;
using EdugameCloud.Lti.Controllers;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed partial class MeetingSetup : IMeetingSetup
    {
        #region Properties

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        private OfficeHoursModel OfficeHoursModel
        {
            get { return IoC.Resolve<OfficeHoursModel>(); }
        }

        private LmsUserModel LmsUserModel
        {
            get { return IoC.Resolve<LmsUserModel>(); }
        }

        private LmsUserParametersModel LmsUserParametersModel
        {
            get { return IoC.Resolve<LmsUserParametersModel>(); }
        }

        private LmsCompanyModel LmsСompanyModel
        {
            get { return IoC.Resolve<LmsCompanyModel>(); }
        }

        private UsersSetup UsersSetup
        {
            get { return IoC.Resolve<UsersSetup>(); }
        }

        private LmsFactory LmsFactory
        {
            get { return IoC.Resolve<LmsFactory>(); }
        }

        private IAdobeConnectUserService AcUserService
        {
            get { return IoC.Resolve<IAdobeConnectUserService>(); }
        }

        private IAdobeConnectAccountService AcAccountService
        {
            get { return IoC.Resolve<IAdobeConnectAccountService>(); }
        }

        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        #endregion

        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public OperationResult DeleteMeeting(
            LmsCompany credentials,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id)
        {
            if (!credentials.CanRemoveMeeting.GetValueOrDefault())
            {
                return OperationResult.Error("Meeting deleting is disabled for this company lms.");
            }

            LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, id);
            if (meeting == null)
            {
                return OperationResult.Error("Meeting not found");
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours && meeting.OfficeHours != null)
            {
                var coursesWithThisOfficeHours = this.LmsCourseMeetingModel.GetAllByOfficeHoursId(
                    meeting.OfficeHours.Id);
                if (coursesWithThisOfficeHours.Any(c => c.Id != meeting.Id))
                {
                    this.LmsCourseMeetingModel.RegisterDelete(meeting);
                    return OperationResult.Success();
                }
            }

            this.LmsCourseMeetingModel.RegisterDelete(meeting, flush: true);
            if (meeting.OfficeHours != null)
            {
                // TODO: test OH delete
                this.OfficeHoursModel.RegisterDelete(meeting.OfficeHours, flush: true);
            }

            bool acMeetingIsStillUsed = this.LmsCourseMeetingModel.ContainsByCompanyAndScoId(credentials, meeting.ScoId);
            if (acMeetingIsStillUsed)
            {
                return OperationResult.Success();
            }

            if (meeting.Reused.GetValueOrDefault() && !meeting.SourceCourseMeetingId.HasValue)
            {
                return OperationResult.Success();
            }

            var result = provider.DeleteSco(meeting.GetMeetingScoId());
            if (result.Code == StatusCodes.ok)
            {
                return OperationResult.Success();
            }

            return OperationResult.Error(result.InnerXml);
        }

        /// <summary>
        /// The get meetings.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDTO"/>.
        /// </returns>
        public object GetMeetings(LmsCompany credentials, IAdobeConnectProxy provider, LtiParamDTO param)
        {
            var ret = new List<MeetingDTO>();
            
            var meetings = this.LmsCourseMeetingModel.GetAllByCourseId(credentials.Id, param.course_id);

            foreach (var meeting in meetings)
            {
                ScoInfoResult result = provider.GetScoInfo(meeting.GetMeetingScoId());
                if (!result.Success || result.ScoInfo == null)
                {
                    continue;
                }

                MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                    provider,
                    param,
                    result.ScoInfo,
                    meeting);
                ret.Add(meetingDTO);
            }
            
            if (!ret.Any(m => m.type == (int)LmsMeetingType.OfficeHours))
            {
                var officeHoursMeetings =
                    this.LmsCourseMeetingModel.GetOneByUserAndType(credentials.Id, param.lms_user_id, LmsMeetingType.OfficeHours).Value;

                if (officeHoursMeetings == null)
                {
                    var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, credentials.Id).Value;
                    if (lmsUser != null)
                    {
                        var officeHours = this.OfficeHoursModel.GetByLmsUserId(lmsUser.Id).Value;
                        if (officeHours != null)
                        {
                            officeHoursMeetings = new LmsCourseMeeting
                            {
                                OfficeHours = officeHours,
                                LmsMeetingType = (int)LmsMeetingType.OfficeHours,
                                LmsCompany = credentials,
                                CourseId = param.course_id,
                            };
                        }
                    }
                }

                if (officeHoursMeetings != null)
                {
                    ScoInfoResult result = provider.GetScoInfo(officeHoursMeetings.GetMeetingScoId());
                    if (result.Success && result.ScoInfo != null)
                    {
                        MeetingDTO meetingDTO = this.GetMeetingDTOByScoInfo(
                            provider,
                            param,
                            result.ScoInfo,
                            officeHoursMeetings);
                        meetingDTO.is_disabled_for_this_course = true;
                        ret.Add(meetingDTO);
                    }
                }
            }
            
            return new
            {
                meetings = ret,
                is_teacher = this.UsersSetup.IsTeacher(param),
                lms_provider_name = credentials.LmsProvider.LmsProviderName,
                connect_server = credentials.AcServer + "/",
                is_settings_visible = credentials.IsSettingsVisible.GetValueOrDefault(),
                is_removable = credentials.CanRemoveMeeting.GetValueOrDefault(),
                can_edit_meeting = credentials.CanEditMeeting.GetValueOrDefault(),
                office_hours_enabled = credentials.EnableOfficeHours.GetValueOrDefault(),
                study_groups_enabled = credentials.EnableStudyGroups.GetValueOrDefault(),
                course_meetings_enabled = credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,
                is_lms_help_visible = credentials.ShowLmsHelp.GetValueOrDefault(),
                is_egc_help_visible = credentials.ShowEGCHelp.GetValueOrDefault(),
                user_guide_link = !string.IsNullOrEmpty(credentials.LmsProvider.UserGuideFileUrl) 
                    ? credentials.LmsProvider.UserGuideFileUrl 
                    : string.Format("/content/lti-instructions/{0}.pdf", credentials.LmsProvider.ShortName),
            };
        }
        
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ACSessionDTO> GetSessionsReport(IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0)
        {
            return GetSessionsWithParticipants(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }
        
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ACSessionParticipantDTO> GetAttendanceReport(IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0)
        {
            return GetAttendanceReport(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }

        public string JoinMeeting(LmsCompany lmsCompany, LtiParamDTO param, int meetingId, 
            ref string breezeSession, IAdobeConnectProxy adobeConnectProvider = null)
        {
            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, param.course_id, meetingId);

            if (currentMeeting == null)
            {
                throw new WarningMessageException(string.Format("No meeting for course {0} and id {1} found.", param.course_id, meetingId));
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();
            IAdobeConnectProxy provider = adobeConnectProvider ?? AcAccountService.GetProvider(lmsCompany, true);
            var meetingUrl = string.Empty;

            if (!string.IsNullOrEmpty(currentMeetingScoId))
            {
                ScoContent currentMeetingSco = provider.GetScoContent(currentMeetingScoId).ScoContent;
                if (currentMeetingSco != null)
                {
                    meetingUrl = (lmsCompany.AcServer.EndsWith("/")
                        ? lmsCompany.AcServer.Substring(0, lmsCompany.AcServer.Length - 1)
                        : lmsCompany.AcServer) + currentMeetingSco.UrlPath;
                }
            }

            string email = param.lis_person_contact_email_primary;
            string login = param.lms_user_login;
            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                throw new WarningMessageException(string.Format("No user with id {0} found.", param.lms_user_id));
            }

            var principalInfo = !string.IsNullOrWhiteSpace(lmsUser.PrincipalId) ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo : null;
            Principal registeredUser = principalInfo != null ? principalInfo.Principal : null;
            
            if (currentMeeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                var userId = currentMeeting.OfficeHours != null && currentMeeting.OfficeHours.LmsUser != null
                                 ? currentMeeting.OfficeHours.LmsUser.UserId
                                 : string.Empty;
                var isOwner = userId.Equals(param.lms_user_id);

                if (registeredUser != null)
                {
                    provider.UpdateScoPermissionForPrincipal(
                        currentMeetingScoId,
                        registeredUser.PrincipalId,
                        isOwner ? MeetingPermissionId.host : MeetingPermissionId.view);
                    if (isOwner)
                    {
                        this.UsersSetup.AddUserToMeetingHostsGroup(provider, registeredUser.PrincipalId);
                    }
                }
            }

            var breezeToken = string.Empty;

            if (registeredUser != null)
            {
                breezeToken = ACLogin(lmsCompany, param, lmsUser, registeredUser, provider);

                string wstoken = null;

                if (lmsCompany.LmsProvider.Id == (int)LmsProviderEnum.Blackboard)
                {
                    string error;

                    var lmsUserService = LmsFactory.GetUserService((LmsProviderEnum) lmsCompany.LmsProvider.Id);
                    var currentUser = lmsUserService.GetUser(lmsCompany, lmsUser, 
                        param.lms_user_id,
                        param.course_id,
                        out error,
                        param);

                    if (currentUser != null)
                    {
                        wstoken = currentUser.id;
                    }
                }

                this.SaveLMSUserParameters(param, lmsCompany, registeredUser.PrincipalId, wstoken);
            }
            else
            {
                var error = string.Format(
                    "Cannot find Adobe Connect user with principal id {0} or email {1} or login {2}.",
                    lmsUser.PrincipalId ?? string.Empty,
                    email,
                    login);
                throw new WarningMessageException(error);
            }

            breezeSession = breezeToken ?? string.Empty;
            return lmsCompany.LoginUsingCookie.GetValueOrDefault() 
                ? meetingUrl 
                : string.Format("{0}{1}", meetingUrl, breezeToken != null ? "?session=" + breezeToken : string.Empty);
        }

        public string ACLogin(LmsCompany lmsCompany, LtiParamDTO param, LmsUser lmsUser,
            Principal registeredUser, IAdobeConnectProxy provider)
        {
            string breezeToken = null;
            string generatedPassword = null;
            if (lmsUser.AcConnectionMode == AcConnectionMode.Overwrite && string.IsNullOrEmpty(lmsUser.ACPassword))
            {
                if (lmsCompany.AcUsername.Equals(registeredUser.Login, StringComparison.OrdinalIgnoreCase))
                {
                    generatedPassword = lmsCompany.AcPassword;
                    UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                }
                else
                { 
                    generatedPassword = Membership.GeneratePassword(8, 2);
                    var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
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
                    lmsCompany,
                    param,
                    registeredUser,
                    password,
                    provider);
                if (breezeToken == null && generatedPassword == null && lmsUser.AcConnectionMode == AcConnectionMode.Overwrite)
                {
                    // trying to login with new generated password (in case, for example, when user changed his AC password manually)
                    generatedPassword = Membership.GeneratePassword(8, 2);
                    var resetPasswordResult = provider.PrincipalUpdatePassword(registeredUser.PrincipalId, generatedPassword);
                    if (resetPasswordResult.Success)
                    {
                        UsersSetup.ResetUserACPassword(lmsUser, generatedPassword);
                    }
                    breezeToken = AcAccountService.LoginIntoAC(
                        lmsCompany,
                        param,
                        registeredUser,
                        generatedPassword,
                        provider);
                }
            }

            return breezeToken;
        }

        public OperationResult LeaveMeeting(LmsCompany lmsCompany, LtiParamDTO param, int id, IAdobeConnectProxy provider)
        {
            this.LmsCourseMeetingModel.Flush();
            LmsCourseMeeting currentMeeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, param.course_id, id);

            if (currentMeeting == null)
            {
                return OperationResult.Error("No meeting found");
            }

            if (currentMeeting.LmsMeetingType != (int)LmsMeetingType.StudyGroup)
            {
                return OperationResult.Error("The meeting is not a study group, you can only leave study groups.");
            }

            string currentMeetingScoId = currentMeeting.GetMeetingScoId();

            string login = param.lms_user_login;
            string email = param.lis_person_contact_email_primary;
            
            Principal registeredUser = AcUserService.GetPrincipalByLoginOrEmail(provider, login, email, lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault());

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

            return OperationResult.Error(string.Format("Cannot find Adobe Connect user with email {0} or login {1}", email, login));
        }



        public OperationResult SaveMeeting(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider, 
            LtiParamDTO param,
            MeetingDTO meetingDTO,
            bool retrieveLmsUsers = false)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (param == null)
                throw new ArgumentNullException("param");
            if (meetingDTO == null)
                throw new ArgumentNullException("meetingDTO");

            FixDateTimeFields(meetingDTO, param);

            LmsUser lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                return OperationResult.Error(string.Format("No lms user found with id={0} and companyLmsId={1}", param.lms_user_id, lmsCompany.Id));
            }

            LmsCourseMeeting meeting = this.GetCourseMeeting(lmsCompany, param.course_id, meetingDTO.id, meetingDTO.GetMeetingType());
            string meetingSco = meeting.GetMeetingScoId();

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

            var updateItem = new MeetingUpdateItem { ScoId = isNewMeeting ? null : meetingSco };

            var currentUserPrincipal = AcUserService.GetPrincipalByLoginOrEmail(
                provider,
                param.lms_user_login,
                param.lis_person_contact_email_primary,
                lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault());

            //===========================================
            var lmsUsers = new List<LmsUserDTO>();
            if (isNewMeeting)
            {
                //NOTE: need to call before use GetMeetingFolder method;
                // when we call group-membership-update api action ac create folder in the user-meetings directory called as user login;
                this.UsersSetup.AddUserToMeetingHostsGroup(provider, currentUserPrincipal.PrincipalId);

                // NOTE: for meeting we need users to add to AC meeting;
                // For StudyGroup - to be sure we can get them on 2nd tab (and reuse them if retrieveLmsUsers==true)
                if ((meeting.LmsMeetingType == (int)LmsMeetingType.Meeting) || (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup))
                {
                    string error;
                    lmsUsers = this.UsersSetup.GetLMSUsers(lmsCompany,
                        meeting,
                        param.lms_user_id,
                        meeting.CourseId,
                        out error,
                        param);
                    if (error != null)
                    {
                        return OperationResult.Error("Unable retrieve information about LMS users.");
                    }
                }
            }
            //===========================================

            var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);

            string meetingFolder = this.GetMeetingFolder(lmsCompany, provider, currentUserPrincipal, useLmsUserEmailForSearch);     
             
            SetMeetingUpdateItemFields(
                meetingDTO,
                updateItem,
                meetingFolder,
                isNewMeeting);

            ProcessMeetingName(lmsCompany, param, meetingDTO, lmsUser,
                isNewMeeting, updateItem, meeting, officeHours);

            ScoInfoResult result = isNewMeeting ? provider.CreateSco(updateItem) : provider.UpdateSco(updateItem);

            if (!result.Success || result.ScoInfo == null)
            {
                if ((result.Status.SubCode == StatusSubCodes.duplicate) && (result.Status.InvalidField == "name"))
                    return OperationResult.Error("There is already another item with this name. Please try again.");

                if ((result.Status.SubCode == StatusSubCodes.duplicate) && (result.Status.InvalidField == "url-path"))
                    return OperationResult.Error("URLs must be unique, and the URL path you chose is already in use. Please select an alternative URL path. If you need additional information, please contact your account administrator.");

                return OperationResult.Error(result.Status.Code.ToString() + " " + result.Status.SubCode.ToString());
            }

            if (isNewMeeting)
            {
                // newly created meeting
                if (meeting.LmsMeetingType != (int)LmsMeetingType.OfficeHours)
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
            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                officeHours = officeHours ?? new OfficeHours { LmsUser = lmsUser };
                officeHours.Hours = meetingDTO.office_hours;
                officeHours.ScoId = meeting.ScoId = result.ScoInfo.ScoId;                    
                this.OfficeHoursModel.RegisterSave(officeHours);

                meeting.OfficeHours = officeHours;
                meeting.ScoId = null;
                attachToExistedOfficeHours = !isNewMeeting && (meeting.Id == 0); // we attach existed office hours meeting for another course
            }
            else if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                meeting.Owner = lmsUser;
            }

            this.LmsCourseMeetingModel.RegisterSave(meeting);
            this.LmsCourseMeetingModel.Flush();

            SpecialPermissionId specialPermissionId = meetingDTO.GetPermissionId();
            provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);

            string message = string.Empty;
            if (isNewMeeting && (meeting.LmsMeetingType == (int)LmsMeetingType.Meeting))
            {
                List<LmsUserDTO> usersToAddToMeeting = this.UsersSetup.GetUsersToAddToMeeting(lmsCompany, lmsUsers, out message);
                
                this.UsersSetup.SetDefaultUsers(
                    lmsCompany,
                    meeting,
                    provider,
                    param.lms_user_id,
                    meeting.CourseId,
                    result.ScoInfo.ScoId,
                    usersToAddToMeeting,
                    param);
            }
                        
            if (isNewMeeting || attachToExistedOfficeHours)
            {
                try
                {
                    CreateAnnouncement(
                        (LmsMeetingType)meeting.LmsMeetingType,
                        lmsCompany,
                        param,
                        meetingDTO);
                }
                catch (Exception)
                {
                    message += "Meetings was created without announcement. Please contact administrator. ";
                }
            }

            MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
                provider,
                param,
                result.ScoInfo,                
                meeting);

            if (retrieveLmsUsers)
            {
                string error;
                var users = this.UsersSetup.GetUsers(lmsCompany,
                    provider,
                    param,
                    updatedMeeting.id,
                    out error,
                    lmsUsers);
                if (error != null)
                {
                    return OperationResult.Error("Unable retrieve information about users.");
                }

		        return OperationResult.Success(message,
                    new MeetingAndLmsUsersDTO
                    {
                        meeting = updatedMeeting,
                        lmsUsers = users,
                    }); ;
	        }

            return OperationResult.Success(message, updatedMeeting);
        }

        // TODO: move MeetingReuseDTO
        public OperationResult ReuseExistedAdobeConnectMeeting(LmsCompany credentials,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            MeetingReuseDTO dto,
            bool retrieveLmsUsers)
        {
            // var param = session.LtiSession.With(x => x.LtiParam);

            ScoInfoResult meetingSco = provider.GetScoInfo(dto.sco_id);
            if (!meetingSco.Success)
            {
                IoC.Resolve<ILogger>().ErrorFormat("[ReuseExistedAdobeConnectMeeting] Meeting not found in Adobe Connect. {0}.", meetingSco.Status.GetErrorInfo());
                return OperationResult.Error("Meeting not found in Adobe Connect");
            }

            LmsCourseMeeting originalMeeting = this.LmsCourseMeetingModel.GetLtiCreatedByCompanyAndScoId(credentials, dto.sco_id);
            int? sourceLtiCreatedMeetingId = (originalMeeting != null) ? originalMeeting.Id : default(int?);
            
            var json = JsonConvert.SerializeObject(new MeetingNameInfo
            {
                reusedMeetingName = meetingSco.ScoInfo.Name,
            });
            
            var meeting = new LmsCourseMeeting
            {
                LmsCompany = credentials,
                CourseId = param.course_id,
                LmsMeetingType = (int)LmsMeetingType.Meeting,
                ScoId = dto.sco_id,
                Reused = true,
                SourceCourseMeetingId = sourceLtiCreatedMeetingId,
                MeetingNameJson = json,
            };

            var lmsUsers = new List<LmsUserDTO>();

            if (retrieveLmsUsers)
            {
                string error;
                lmsUsers = this.UsersSetup.GetLMSUsers(credentials,
                        meeting,
                        param.lms_user_id,
                        meeting.CourseId,
                        out error,
                        param);
                if (error != null)
                {
                    return OperationResult.Error("Unable retrieve information about LMS users.");
                }
            }

            if (originalMeeting != null)
            {
                MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(originalMeeting.MeetingNameJson)
                    ? new MeetingNameInfo()
                    : JsonConvert.DeserializeObject<MeetingNameInfo>(originalMeeting.MeetingNameJson);

                nameInfo.reusedMeetingName = meetingSco.ScoInfo.Name;
                originalMeeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);
                LmsCourseMeetingModel.RegisterSave(originalMeeting);
            }
            LmsCourseMeetingModel.RegisterSave(meeting);
            LmsCourseMeetingModel.Flush();

            if (!dto.mergeUsers)
            {
                // NOTE: Clean all existed AC meeting participants.
                PermissionCollectionResult allMeetingEnrollments = provider.GetAllMeetingEnrollments(dto.sco_id);
                
                provider.UpdateScoPermissionForPrincipal(
                    allMeetingEnrollments.Values.Select(
                        enrollment =>
                            new PermissionUpdateTrio
                            {
                                ScoId = dto.sco_id,
                                PrincipalId = enrollment.PrincipalId,
                                PermissionId = MeetingPermissionId.remove,
                            }));
            }

            this.UsersSetup.SetDefaultUsers(
                credentials,
                meeting,
                provider,
                param.lms_user_id,
                meeting.CourseId,
                meeting.ScoId,
                lmsUsers,
                param);

            MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
                provider,
                param,
                meetingSco.ScoInfo,
                meeting);

            CreateAnnouncement(
                    (LmsMeetingType)meeting.LmsMeetingType,
                    credentials,
                    param,
                    updatedMeeting);

            if (retrieveLmsUsers)
            {
                string error;
                var users = this.UsersSetup.GetUsers(credentials,
                    provider,
                    param,
                    updatedMeeting.id,
                    out error,
                    lmsUsers);
                if (error != null)
                {
                    return OperationResult.Error("Unable retrieve information about users.");
                }

                return OperationResult.Success(
                    new MeetingAndLmsUsersDTO
                    {
                        meeting = updatedMeeting,
                        lmsUsers = users,
                    });
            }

            return OperationResult.Success(updatedMeeting);
        }

        public List<string> DeleteMeeting(
            LmsCompany credentials,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id,
            out string error)
        {
            error = null;
            var model = this.LmsCourseMeetingModel;
            LmsCourseMeeting meeting = model.GetOneByCourseAndId(credentials.Id, param.course_id, id);

            if (meeting == null)
            {
                error = "Meeting not found";
                return new List<string>();
            }

            List<PermissionInfo> enrollments = this.UsersSetup.GetMeetingAttendees(provider, meeting.GetMeetingScoId());
            model.RegisterDelete(meeting, true);

            bool acMeetingIsStillUsed = this.LmsCourseMeetingModel.ContainsByCompanyAndScoId(credentials, meeting.GetMeetingScoId());
            if (!acMeetingIsStillUsed)
            {
                if  (!meeting.Reused.GetValueOrDefault() || (meeting.Reused.GetValueOrDefault() && meeting.SourceCourseMeetingId.HasValue))
                    provider.DeleteSco(meeting.GetMeetingScoId());
            }

            return enrollments.Select(x => x.Login).ToList();
        }

        /// <summary>
        /// The setup folders.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void SetupFolders(LmsCompany credentials, IAdobeConnectProxy provider)
        {
            SetupTemplateFolder(credentials, provider);
            
            this.LmsСompanyModel.RegisterSave(credentials);
            this.LmsСompanyModel.Flush();   
        }

        /// <summary>
        /// The get meeting folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetMeetingFolder(LmsCompany credentials, IAdobeConnectProxy provider, Principal user, bool useLmsUserEmailForSearch)
        {
            string adobeConnectScoId = null;

            if (credentials.UseUserFolder.GetValueOrDefault() && user != null)
            {
                ////TODO Think about user folders + renaming directory
                adobeConnectScoId = SetupUserMeetingsFolder(credentials, provider, user, useLmsUserEmailForSearch);
            }

            if (adobeConnectScoId == null)
            {
                SetupSharedMeetingsFolder(credentials, provider);
                this.LmsСompanyModel.RegisterSave(credentials);
                this.LmsСompanyModel.Flush();
                adobeConnectScoId = credentials.ACScoId;
            }

            return adobeConnectScoId;
        }
        
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
                    // TODO: add to LOg
                    var acServer = cm.Return(c => c.LmsCompany.Return(cl => cl.AcServer, null), null);
                    if (acServer == null)
                    {
                        return false;
                    }

                    if (acServer.EndsWith("/"))
                    {
                        acServer = acServer.Substring(0, acServer.Length - 1);
                    }
                    // TODO: add to LOg
                    return acDomain.StartsWith(acServer, StringComparison.InvariantCultureIgnoreCase);
                })
                    .ToList();

            if (!serverCourseMeetings.Any())
            {
                error = "This meeting is not associated to any course";
                return null;
            }

            var paramList = new List<LmsUserParameters>();

            foreach (var courseMeeting in serverCourseMeetings)
            {
                if (courseMeeting.LmsCompany == null)
                {
                    continue;
                }

                var param = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(acId, courseMeeting.CourseId, courseMeeting.LmsCompany.Id).Value;
                if (param != null)
                {
                    paramList.Add(param);
                }
            }

            return paramList.Any() ? new LmsUserParametersDTO(paramList.OrderByDescending(p => p.LastLoggedIn ?? "0").First()) : null;
        }

        #endregion

        #region Methods
        
        private void ProcessMeetingName(LmsCompany lmsCompany, LtiParamDTO param, MeetingDTO meetingDTO, LmsUser lmsUser,
            bool isNewMeeting, MeetingUpdateItem updateItem, LmsCourseMeeting meeting, OfficeHours officeHours)
        {
            string courseId = meetingDTO.GetMeetingType() == LmsMeetingType.OfficeHours
                ? lmsUser.Id.ToString(CultureInfo.InvariantCulture)
                : param.course_id.ToString(CultureInfo.InvariantCulture);

            int formatterId = lmsCompany.MeetingNameFormatterId;
            IMeetingNameFormatter formatter = MeetingNameFormatterFactory.GetFormatter(formatterId);

            // officeHours - map existing AC meeting but new LmsCourseMeeting record
            if (isNewMeeting)
            {
                string acMeetingName = formatter.BuildName(meetingDTO, param, courseId);
                updateItem.Name = acMeetingName;

                // TODO: move TO formatter base?
                var json = JsonConvert.SerializeObject(new MeetingNameInfo
                {
                    courseId = courseId,
                    courseNum = param.context_label,
                    meetingName = meetingDTO.name,
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
                var json = JsonConvert.SerializeObject(new MeetingNameInfo
                {
                    courseId = courseId,
                    courseNum = param.context_label,
                    meetingName = meetingDTO.name,
                    date = DateTime.Today.ToString("MM/dd/yy"),
                });
                meeting.MeetingNameJson = json;
            }
            else
            {
                // TODO: is it OK to use the setting here? if (lmsCompany.EnableMeetingReuse)
                var thisScoMeetings = LmsCourseMeetingModel.GetByCompanyAndScoId(lmsCompany, meeting.GetMeetingScoId(), meeting.Id);
                
                // NOTE: it's reused meeting or source for any reused meeting
                //if (!string.IsNullOrEmpty(nameInfo.reusedMeetingName) || meeting.Reused.GetValueOrDefault())
                if (thisScoMeetings.Any())
                {
                    //MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
                    //nameInfo.reusedMeetingName = meetingDTO.name;
                    //meeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);
                    updateItem.Name = meetingDTO.name;

                    // NOTE: find original LTI meeting and all its reusings; and change their name AS WELL
                    //var thisScoMeetings = LmsCourseMeetingModel.GetByCompanyAndScoId(lmsCompany, meeting.GetMeetingScoId());
                    foreach (LmsCourseMeeting m in thisScoMeetings)
                    {
                        MeetingNameInfo name = JsonConvert.DeserializeObject<MeetingNameInfo>(m.MeetingNameJson);
                        name.reusedMeetingName = meetingDTO.name;
                        m.MeetingNameJson = JsonConvert.SerializeObject(name);
                        LmsCourseMeetingModel.RegisterSave(m);
                    }
                    MeetingNameInfo currentMeetingName = JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
                    currentMeetingName.reusedMeetingName = meetingDTO.name;
                    meeting.MeetingNameJson = JsonConvert.SerializeObject(currentMeetingName);
                    LmsCourseMeetingModel.RegisterSave(meeting);
                }
                else
                {
                    string acMeetingName = formatter.UpdateName(meeting, meetingDTO.name);
                    updateItem.Name = acMeetingName;
                }
            }

            if (!isNewMeeting && (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours))
            {
                var coursesWithThisOfficeHours = this.LmsCourseMeetingModel.GetAllByOfficeHoursId(officeHours.Id);
                foreach (LmsCourseMeeting officeHoursMeeting in coursesWithThisOfficeHours.
                    Where(x => !string.IsNullOrWhiteSpace(x.MeetingNameJson)))
                {
                    MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(officeHoursMeeting.MeetingNameJson);
                    nameInfo.meetingName = meetingDTO.name;
                    officeHoursMeeting.MeetingNameJson = JsonConvert.SerializeObject(nameInfo);
                    LmsCourseMeetingModel.RegisterSave(officeHoursMeeting);
                }
            }
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

        /// <summary>
        /// The save LMS user parameters.
        /// </summary>
        /// <param name="lmsCourseId">
        /// The LMS Course Id.
        /// </param>
        /// <param name="lmsCompany">
        /// The LMS Company.
        /// </param>
        /// <param name="lmsUserId">
        /// The LMS User Id.
        /// </param>
        /// <param name="adobeConnectUserId">
        /// The current user AC id.
        /// </param>
        /// <param name="courseName">
        /// The course Name.
        /// </param>
        /// <param name="userEmail">
        /// The user Email.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        private void SaveLMSUserParameters(
            int lmsCourseId,
            LmsCompany lmsCompany,
            string lmsUserId,
            string adobeConnectUserId,
            string courseName,
            string userEmail,
            string userId)
        {
            LmsUserParameters lmsUserParameters = this.LmsUserParametersModel.GetOneByAcIdCourseIdAndCompanyLmsId(adobeConnectUserId, lmsCourseId, lmsCompany.Id).Value;

            if (lmsUserParameters == null)
            {
                lmsUserParameters = new LmsUserParameters
                {
                    AcId = adobeConnectUserId,
                    Course = lmsCourseId,
                    CompanyLms = lmsCompany
                };
            }

            lmsUserParameters.Wstoken = userId;
            lmsUserParameters.LastLoggedIn = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ");
            lmsUserParameters.CourseName = courseName;
            lmsUserParameters.UserEmail = userEmail;
            lmsUserParameters.LmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
            this.LmsUserParametersModel.RegisterSave(lmsUserParameters);
        }

        /// <summary>
        /// The can edit.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanEdit(LtiParamDTO param, LmsCourseMeeting meeting)
        {
            if (meeting == null)
            {
                return false;
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
            {
                return meeting.OfficeHours != null && meeting.OfficeHours.LmsUser != null
                        && meeting.OfficeHours.LmsUser.UserId != null
                       && meeting.OfficeHours.LmsUser.UserId.Equals(param.lms_user_id);
            }

            if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
            {
                return meeting.Owner != null && meeting.Owner.UserId != null && meeting.Owner.UserId.Equals(param.lms_user_id);
            }

            return this.UsersSetup.IsTeacher(param);
        }

        /// <summary>
        /// The can join.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="lmsCompany">
        /// The company LMS.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="meetingSco">
        /// The meeting SCO.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanJoin(
            IAdobeConnectProxy provider,
            LmsCompany lmsCompany,
            int type,
            LtiParamDTO param,
            string meetingSco)
        {
            if (type == (int)LmsMeetingType.OfficeHours)
            {
                return true;
            }

            var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            
            // this method is called after the user has opened the application through LtiController, so there should already be Principal found and saved for the user.
            if ((lmsUser == null) || string.IsNullOrWhiteSpace(lmsUser.PrincipalId))
            {
                return false;
            }

            var enrollments = this.UsersSetup.GetMeetingAttendees(provider, meetingSco);
            
            return enrollments.Any(e => e.PrincipalId != null && e.PrincipalId.Equals(lmsUser.PrincipalId));
        }
        
        /// <summary>
        /// The get attendance Report.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting Id.
        /// </param>
        /// <param name="acp">
        /// The ACP.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List{ACSessionParticipantDTO}"/>.
        /// </returns>
        private static List<ACSessionParticipantDTO> GetAttendanceReport(string meetingId, IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                var meetingAttendees = acp.ReportMettingAttendance(meetingId, startIndex, limit).Values.ToList();
                return meetingAttendees.Select(
                        us =>
                        new ACSessionParticipantDTO
                        {
                            scoId = us.ScoId,
                            scoName = us.ScoName,
                            participantName = us.ParticipantName,
                            assetId = us.AssetId,
                            sessionName = us.SessionName,
                            principalId = us.PrincipalId,
                            firstName = us.SessionName,
                            login = us.Login,
                            dateTimeEntered = us.DateCreated,
                            dateTimeLeft = us.DateEnd.FixACValue(),
                            durationInHours = (float)us.Duration.TotalHours,
                            transcriptId = int.Parse(us.TranscriptId)
                        }).OrderByDescending(x => x.dateTimeEntered).ToList();
                
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("GetAttendanceReport.Exception", ex);
            }

            return new List<ACSessionParticipantDTO>();
        }
        
        private static List<ACSessionDTO> GetSessionsWithParticipantsBySessionTime(string meetingId, List<MeetingAttendee> meetingAttendees,
            IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            List<MeetingSession> sessions = acp.ReportMettingSessions(meetingId, startIndex, limit).Values.ToList();
            var result = sessions.Select(sco => new ACSessionDTO
            {
                scoId = int.Parse(sco.ScoId),
                assetId = int.Parse(sco.AssetId),
                dateStarted = sco.DateCreated.FixACValue(),
                dateEnded = sco.DateEnd.FixACValue(),
                sessionNumber = int.Parse(sco.Version),
                sessionName = sco.Version,
                participants = new List<ACSessionParticipantDTO>()
            }).ToList();

            var unprocessedAttendees = new List<MeetingAttendee>();
            foreach (var attendee in meetingAttendees)
            {
                var session = result.FirstOrDefault(x => x.dateStarted <= attendee.DateCreated && (!x.dateEnded.HasValue || attendee.DateEnd <= x.dateEnded));
                if (session != null)
                {
                    session.participants.Add(
                        new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float) attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        });
                    if (String.IsNullOrEmpty(session.meetingName))
                    {
                        session.meetingName = attendee.ScoName;
                    }
                }
                else
                {
                    unprocessedAttendees.Add(attendee);
                }
            }
            //unlikely possible case, need to check
            if (unprocessedAttendees.Count > 0)
            {
                var maxDate = result.Max(x => x.dateStarted);
                var ua = unprocessedAttendees.Where(x => x.DateCreated >= maxDate);
                if (ua.Any())
                {
                    var currentSessionNumber = result.Max(x => x.sessionNumber);
                    result.Add(new ACSessionDTO
                    {
                        sessionNumber = currentSessionNumber + 1,
                        sessionName = (currentSessionNumber + 1).ToString(CultureInfo.CurrentCulture),
                        dateStarted = ua.Min(x => x.DateCreated),
                        participants = ua.Select(attendee => new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float)attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        }).ToList(),
                        meetingName = ua.First().ScoName
                    });
                }
            }

            GroupSessionParticipants(result);
            return result.OrderBy(s => s.sessionNumber).ToList();
        }

        private static List<ACSessionDTO> GetSessionsWithParticipants(string meetingId, IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                List<MeetingAttendee> meetingAttendees = acp.ReportMettingAttendance(meetingId).Values.ToList();
                if (meetingAttendees.All(x => string.IsNullOrEmpty(x.AssetId)))
                {
                    //todo: we should not rely on AssetId parameter and probably use following method in all cases
                    return GetSessionsWithParticipantsBySessionTime(meetingId, meetingAttendees, acp, startIndex, limit);
                }

                //left previous version to avoid any possible errors
                var userSessions = meetingAttendees.Where(x => !string.IsNullOrEmpty(x.AssetId))
                    .GroupBy(v => v.AssetId, v => v)
                    .ToDictionary(g => int.Parse(g.Key), g => g.ToList());

                var sessions = acp.ReportMettingSessions(meetingId, startIndex, limit).Values.ToList();

                var sessionList =
                    (from asset in userSessions.Keys.Except(sessions.ConvertAll(s => int.Parse(s.AssetId)))
                        let index =
                            sessions.Any(s => !string.IsNullOrEmpty(s.Version))
                                ? sessions.Max(s => int.Parse(s.Version)) + 1
                                : 0
                        select
                            new ACSessionDTO
                            {
                                assetId = asset,
                                sessionNumber = index,
                                sessionName = index.ToString(CultureInfo.CurrentCulture)
                            }).ToList();
                sessions.AddRange(
                    sessionList.Select(
                        s => new MeetingSession { AssetId = s.assetId.ToString(CultureInfo.CurrentCulture) }));

                foreach (var sco in sessions)
                {
                    var session = sessionList.FirstOrDefault(s => s.assetId == int.Parse(sco.AssetId));
                    if (null == session)
                    {
                        session = new ACSessionDTO
                        {
                            scoId = int.Parse(sco.ScoId),
                            assetId = int.Parse(sco.AssetId),
                            dateStarted = sco.DateCreated.FixACValue(),
                            dateEnded = sco.DateEnd.FixACValue(),
                            sessionNumber = int.Parse(sco.Version),
                            sessionName = sco.Version,
                            participants = new List<ACSessionParticipantDTO>(),
                        };

                        sessionList.Add(session);
                    }

                    foreach (var us in userSessions[session.assetId])
                    {
                        var participant = new ACSessionParticipantDTO
                        {
                            firstName = us.SessionName,
                            login = us.Login,
                            dateTimeEntered = us.DateCreated,
                            dateTimeLeft = us.DateEnd.FixACValue(),
                            durationInHours =
                                (float) us.Duration.TotalHours,
                            transcriptId = int.Parse(us.TranscriptId),
                        };

                        session.meetingName = us.ScoName;
                        session.participants.Add(participant);
                    }

                    if (!session.dateStarted.HasValue)
                    {
                        session.dateStarted = session.participants.Min(p => p.dateTimeEntered);
                    }
                }

                GroupSessionParticipants(sessionList);

                return sessionList.OrderBy(s => s.sessionNumber).ToList();
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("GetSessionsWithParticipants.Exception", ex);
            }

            return new List<ACSessionDTO>();
        }

        private static void GroupSessionParticipants(IEnumerable<ACSessionDTO> sessionList)
        {
            foreach (var session in sessionList)
            {
                var singleAttendance = session.participants.GroupBy(p => p.loginOrFullName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var attendance in singleAttendance.Where(a => !string.IsNullOrWhiteSpace(a.Key) && a.Value.Count > 1))
                {
                    attendance.Value.Skip(1).ToList().ForEach(p => session.participants.Remove(p));
                    var attendee = attendance.Value.First();
                    attendee.dateTimeEntered = attendance.Value.Min(p => p.dateTimeEntered);
                    attendee.dateTimeLeft = attendance.Value.Max(p => p.dateTimeLeft);
                    attendee.durationInHours = attendance.Value.Sum(p => p.durationInHours);
                }
            }
        }

        private MeetingDTO GetMeetingDTOByScoInfo(
            IAdobeConnectProxy provider, 
            LtiParamDTO param, 
            ScoInfo meetingSco,
            LmsCourseMeeting lmsCourseMeeting)
        {
            LmsCompany lmsCompany = lmsCourseMeeting.LmsCompany;
            IEnumerable<PermissionInfo> permission = provider.GetScoPublicAccessPermissions(lmsCourseMeeting.GetMeetingScoId()).Values;

            bool isEditable = this.CanEdit(param, lmsCourseMeeting);
            var type = lmsCourseMeeting.LmsMeetingType;
            var canJoin = this.CanJoin(provider, lmsCompany, type, param, meetingSco.ScoId);
            PermissionInfo permissionInfo = permission != null ? permission.FirstOrDefault() : null;
            string officeHoursString = null;

            if (type == (int)LmsMeetingType.OfficeHours)
            {
                officeHoursString = lmsCourseMeeting.OfficeHours.Hours;
            }

            string meetingName = string.Empty;
            // NOTE: support created meetings; update MeetingNameJson
            if (string.IsNullOrWhiteSpace(lmsCourseMeeting.MeetingNameJson))
            {
                int bracketIndex = meetingSco.Name.IndexOf("]", StringComparison.Ordinal);
                meetingName = meetingSco.Name.Substring(bracketIndex < 0 || (bracketIndex + 2 > meetingSco.Name.Length) ? 0 : bracketIndex + 2);

                string js = JsonConvert.SerializeObject(new MeetingNameInfo
                {
                    courseId = param.course_id.ToString(),
                    courseNum = param.context_label,
                    meetingName = meetingName,
                    date = meetingSco.DateCreated.ToString("MM/dd/yy"),
                });

                lmsCourseMeeting.MeetingNameJson = js;
                LmsCourseMeetingModel.RegisterSave(lmsCourseMeeting, flush: true);
            }
            else
            {
                MeetingNameInfo nameInfo = JsonConvert.DeserializeObject<MeetingNameInfo>(lmsCourseMeeting.MeetingNameJson);
                // NOTE: it is reused meeting or source of reusing
                if (!string.IsNullOrWhiteSpace(nameInfo.reusedMeetingName))
                {
                    meetingName = nameInfo.reusedMeetingName;
                }
                else
                {
                    meetingName = nameInfo.meetingName;
                }
            }

            bool scoIdReused = (lmsCourseMeeting.Reused.HasValue && lmsCourseMeeting.Reused.Value) 
                || LmsCourseMeetingModel.GetByCompanyAndScoId(lmsCompany, lmsCourseMeeting.GetMeetingScoId(), lmsCourseMeeting.Id).Any();

            var ret = new MeetingDTO
            {
                id = lmsCourseMeeting.Id,
                ac_room_url = meetingSco.UrlPath.Trim("/".ToCharArray()),
                name = meetingName,
                summary = meetingSco.Description, 
                template = meetingSco.SourceScoId, 
                start_date = meetingSco.BeginDate.ToString("yyyy-MM-dd"), 
                start_time = meetingSco.BeginDate.ToString("h:mm tt", CultureInfo.InvariantCulture), 
                duration = (meetingSco.EndDate - meetingSco.BeginDate).ToString(@"h\:mm"),
                access_level = permissionInfo != null ? permissionInfo.PermissionId.ToString() : "remove",
                allow_guests = permissionInfo == null || permissionInfo.PermissionId == PermissionId.remove,
                can_join = canJoin,
                is_editable = isEditable,
                type = type,
                office_hours = officeHoursString,
                reused = scoIdReused,
            };
            return ret;
        }

        /// <summary>
        /// The save LMS user parameters.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="lmsCompany">
        /// The LMS Company.
        /// </param>
        /// <param name="adobeConnectUserId">
        /// The current AC user SCO id.
        /// </param>
        private void SaveLMSUserParameters(LtiParamDTO param, LmsCompany lmsCompany, string adobeConnectUserId, string wstoken)
        {
            this.SaveLMSUserParameters(param.course_id, lmsCompany, param.lms_user_id, adobeConnectUserId, param.context_title, param.lis_person_contact_email_primary, wstoken);
        }

        private static void FixDateTimeFields(MeetingDTO meetingDTO, LtiParamDTO param)
        {
            if (meetingDTO.start_time != null)
            {
                meetingDTO.start_time = meetingDTO.start_time.PadLeft(8, '0');
            }

            if (meetingDTO.start_date != null)
            {
                meetingDTO.start_date = meetingDTO.start_date.Substring(6, 4) + "-"
                                        + meetingDTO.start_date.Substring(0, 5);
            }
        }

        public LmsCourseMeeting GetCourseMeeting(LmsCompany lmsCompany, int courseId, int id, LmsMeetingType type)
        {
            LmsCourseMeeting meeting = null;

            if (id > 0)
                meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, courseId, id);

            // TRICK: we should always pass ID from client if it's UPDATE operation for meeting within the course
            //if (meeting == null && type == LmsMeetingType.OfficeHours)
            //{
            //    meeting =
            //        this.LmsCourseMeetingModel.GetOneByCourseAndType(
            //            lmsCompany.Id,
            //            courseId,
            //            LmsMeetingType.OfficeHours).Value;
            //}

            if (meeting == null)
            {
                meeting = new LmsCourseMeeting
                {
                    LmsCompany = lmsCompany,
                    CourseId = courseId,
                    LmsMeetingType = (int)type,
                    // HACK: IMPLEMENT
                    //ScoId = type == LmsMeetingType.OfficeHours ? scoId : null,
                };
            }

            return meeting;
        }

        private static void SetMeetingUpdateItemFields(
            MeetingDTO meetingDTO, 
            MeetingUpdateItem updateItem, 
            string folderSco, 
            bool isNew)
        {
            updateItem.Description = meetingDTO.summary;
            updateItem.FolderId = folderSco;
            updateItem.Language = "en";
            updateItem.Type = ScoType.meeting;
            
            if (isNew)
            {
                updateItem.SourceScoId = meetingDTO.template;
                updateItem.UrlPath = meetingDTO.ac_room_url;
            }

            if (meetingDTO.start_date == null || meetingDTO.start_time == null)
            {
                updateItem.DateBegin = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                updateItem.DateEnd = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            DateTime dateBegin;

            if (DateTime.TryParse(meetingDTO.start_date + " " + meetingDTO.start_time, out dateBegin))
            {
                updateItem.DateBegin = dateBegin.ToString("yyyy-MM-ddTHH:mm:ssZ");
                TimeSpan duration;
                if (TimeSpan.TryParse(meetingDTO.duration, out duration))
                {
                    updateItem.DateEnd =
                        dateBegin.AddMinutes((int)duration.TotalMinutes).ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
            }
        }

        /// <summary>
        /// The setup template folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private static void SetupTemplateFolder(LmsCompany credentials, IAdobeConnectProxy provider)
        {
            string templatesSco = null;
            if (!string.IsNullOrWhiteSpace(credentials.ACTemplateScoId))
            {
                ScoInfoResult templatesFolder = provider.GetScoInfo(credentials.ACTemplateScoId);
                if (templatesFolder.Success && templatesFolder.ScoInfo != null)
                {
                    templatesSco = templatesFolder.ScoInfo.ScoId;
                }
            }

            if (templatesSco == null)
            {
                ScoContentCollectionResult sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
                if (sharedTemplates.ScoId != null)
                {
                    credentials.ACTemplateScoId = sharedTemplates.ScoId;
                }
            }
        }

        /// <summary>
        /// The setup shared meetings folder.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private static void SetupSharedMeetingsFolder(LmsCompany credentials, IAdobeConnectProxy provider)
        {
            string ltiFolderSco = null;
            string name = credentials.UserFolderName ?? credentials.LmsProvider.LmsProviderName;
            name = name.TruncateIfMoreThen(60);
            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null)
                {
                    if (canvasFolder.ScoInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        ltiFolderSco = canvasFolder.ScoInfo.ScoId;
                    }
                    else
                    {
                        ScoInfoResult updatedSco =
                            provider.UpdateSco(
                                new FolderUpdateItem
                                    {
                                        ScoId = canvasFolder.ScoInfo.ScoId,
                                        Name = name,
                                        FolderId = canvasFolder.ScoInfo.FolderId,
                                        Type = ScoType.folder
                                    });
                        if (updatedSco.Success && updatedSco.ScoInfo != null)
                        {
                            ltiFolderSco = updatedSco.ScoInfo.ScoId;
                        }
                    }
                }
            }

            if (ltiFolderSco == null)
            {
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);
                    if (existingFolder != null)
                    {
                        credentials.ACScoId = existingFolder.ScoId;
                    }
                    else
                    {
                        ScoInfoResult newFolder = provider.CreateSco(new FolderUpdateItem { Name = name, FolderId = sharedMeetings.ScoId, Type = ScoType.folder });
                        if (newFolder.Success && newFolder.ScoInfo != null)
                        {
                            provider.UpdatePublicAccessPermissions(newFolder.ScoInfo.ScoId, SpecialPermissionId.denied);
                            credentials.ACScoId = newFolder.ScoInfo.ScoId;
                        }
                    }
                }
            }
        }

        private static void CreateUserFoldersStructure(string folderScoId, IAdobeConnectProxy provider, 
            string userFolderName,
            string userMeetingsFolderName,
            out string innerFolderScoId)
        {
            var folderContent = provider.GetContentsByScoId(folderScoId);
            var userFolder = folderContent.Values.FirstOrDefault(x => x.Name == userFolderName);
            if (userFolder == null)
            {
                var userFolderScoId = CreateFolder(folderScoId, userFolderName, provider);
                var userMeetingsFolderScoId = CreateFolder(userFolderScoId, userMeetingsFolderName, provider);
                innerFolderScoId = userMeetingsFolderScoId;
                return;
            }

            var userFolderContent = provider.GetContentsByScoId(userFolder.ScoId);
            var userMeetingsFolder = userFolderContent.Values.FirstOrDefault(x => x.Name == userMeetingsFolderName);
            if (userMeetingsFolder == null)
            {
                innerFolderScoId =  CreateFolder(userFolder.ScoId, userMeetingsFolderName, provider);
                return;
            }

            innerFolderScoId = userMeetingsFolder.ScoId;
        }

        private static string CreateFolder(string folderScoId, string folderName, IAdobeConnectProxy provider)
        {
            var newFolder = provider.CreateSco(new FolderUpdateItem
            {
                Name = folderName.TruncateIfMoreThen(60),
                FolderId = folderScoId,
                Type = ScoType.folder
            });

            if(!newFolder.Success)
            {
                var msg =string.Format("[AdobeConnectProxy Error] CreateSco " + "Parameters: FolderId:{0}, Name:{1}", folderScoId, folderName);
                throw new InvalidOperationException(msg);
            }
            return newFolder.ScoInfo.ScoId;

        }
        
        private static string SetupUserMeetingsFolder(LmsCompany credentials, IAdobeConnectProxy provider,
            Principal user, bool useLmsUserEmailForSearch)
        {
            var shortcut = provider.GetShortcutByType("user-meetings");

            var userFolderName = useLmsUserEmailForSearch ? user.Email : user.Login;
            var meetingsFolderName = string.IsNullOrEmpty(credentials.UserFolderName)
                ? userFolderName
                : credentials.UserFolderName;
            string meetingFolderScoId;
            CreateUserFoldersStructure(shortcut.ScoId, provider, userFolderName,
                meetingsFolderName, out meetingFolderScoId);
            return meetingFolderScoId;
        }
        #endregion

    }

}
