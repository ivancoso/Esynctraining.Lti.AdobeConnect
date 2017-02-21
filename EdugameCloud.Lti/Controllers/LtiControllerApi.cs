namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Http;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    
    [RoutePrefix("")]
    public class LtiApiController : BaseApiController
    {
        [DataContract]
        public class MeetingRequestDtoEx : MeetingRequestDto
        {
            [DataMember]
            public bool forceUpdate { get; set; }
        }

        #region Fields

        private readonly LmsUserModel lmsUserModel;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;

        #endregion

        private ISynchronizationUserService SynchronizationUserService => IoC.Resolve<ISynchronizationUserService>();

        private LmsFactory LmsFactory => IoC.Resolve<LmsFactory>();

        #region Constructors and Destructors

        public LtiApiController(
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectAccountService acAccountService,
            ILogger logger,
            ICache cache) :base(userSessionModel, acAccountService, settings, logger, cache)
        {
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.usersSetup = usersSetup;
        }

        #endregion
        
        #region Public Methods and Operators
        
        [Route("settings/save")]
        [HttpPost]
        public virtual OperationResultWithData<LmsUserSettingsDTO> SaveSettings([FromBody]LmsUserSettingsDTO settings)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var lmsProviderName = settings.lmsProviderName;
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    lmsUser = session.LmsUser ?? new LmsUser { LmsCompany = lmsCompany, UserId = param.lms_user_id, Username = GetUserNameOrEmail(param) };
                }

                var acConnectionMode = (AcConnectionMode)settings.acConnectionMode;
                lmsUser.PrimaryColor = settings.primaryColor;

                if (acConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
                {
                    var provider = GetAdobeConnectProvider(lmsCompany);
                    var couldSavePassword = usersSetup.SetACPassword(provider, lmsCompany, lmsUser, param, settings.password);
                    if (!couldSavePassword)
                    {
                        return OperationResultWithData<LmsUserSettingsDTO>.Error(Resources.Messages.IncorrectAcPassword);
                    }
                }
                else
                {
                    lmsUser.SharedKey = null;
                    lmsUser.ACPasswordData = null;
                }

                lmsUser.AcConnectionMode = acConnectionMode;
                this.lmsUserModel.RegisterSave(lmsUser);
                return settings.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSettings", lmsCompany, ex);
                return OperationResultWithData<LmsUserSettingsDTO>.Error(errorMessage);
            }
        }
        
        [Route("settings/checkpass")]
        [HttpPost]
        public virtual OperationResultWithData<bool> CheckPasswordBeforeJoin([FromBody]RequestDto request)
        {
            LmsCompany companyLms = null;
            try
            {
                bool isValid = false;
                var session = GetReadOnlySession(request.lmsProviderName);
                companyLms = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
                if (lmsUser != null)
                {
                    var mode = lmsUser.AcConnectionMode;
                    switch (mode)
                    {
                        case AcConnectionMode.DontOverwriteLocalPassword:
                            isValid = !string.IsNullOrWhiteSpace(lmsUser.ACPassword);
                            break;
                        default:
                            isValid = true;
                            break;
                    }
                }

                return isValid.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CheckPasswordBeforeJoin", companyLms, ex);
                return OperationResultWithData<bool>.Error(errorMessage);
            }
        }

        [Route("users")]
        [HttpPost]
        public virtual OperationResultWithData<IList<LmsUserDTO>> GetUsers([FromBody]MeetingRequestDtoEx request)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error;
                var service = LmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProviderId);

                if (request.forceUpdate && lmsCompany.UseSynchronizedUsers
                    && service != null
                    && service.CanRetrieveUsersFromApiForCompany(lmsCompany)
                    && lmsCompany.LmsCourseMeetings != null)
                {
                    SynchronizationUserService.SynchronizeUsers(lmsCompany, syncACUsers: false, meetingIds: new[] { request.meetingId });
                }

                IList<LmsUserDTO> users = this.usersSetup.GetUsers(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param,
                    request.meetingId,
                    out error,
                    null);

                if (string.IsNullOrWhiteSpace(error))
                {
                    return users.ToSuccessResult();
                }

                return OperationResultWithData<IList<LmsUserDTO>>.Error(error);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetUsers", lmsCompany, ex);
                return OperationResultWithData<IList<LmsUserDTO>>.Error(errorMessage);
            }
        }
        
        /// <summary>
        /// The leave meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [Route("meeting/Leave")]
        [HttpPost]
        public virtual OperationResult LeaveMeeting([FromBody]MeetingRequestDto request)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.LeaveMeeting(credentials, param, request.meetingId, this.GetAdobeConnectProvider(credentials));

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("LeaveMeeting", credentials, ex);
                return OperationResult.Error(errorMessage);
            }
        }
        
        [Route("meeting/SetDefaultACRoles")]
        [HttpPost]
        public virtual OperationResultWithData<List<LmsUserDTO>> SetDefaultRolesForNonParticipants([FromBody]MeetingRequestDto request)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error = null;
                List<LmsUserDTO> updatedUsers = this.usersSetup.SetDefaultRolesForNonParticipants(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    request.meetingId,
                    out error);

                //if (string.IsNullOrEmpty(error))
                    return OperationResultWithData<List<LmsUserDTO>>.Success(error, updatedUsers);

                //return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SetDefaultRolesForNonParticipants", credentials, ex);
                return OperationResultWithData<List<LmsUserDTO>>.Error(errorMessage);
            }
        }

        #endregion

        #region Methods
        
        private static string GetUserNameOrEmail(LtiParamDTO param)
        {
            return string.IsNullOrWhiteSpace(param.lms_user_login) ? param.lis_person_contact_email_primary : param.lms_user_login;
        }

        #endregion

    }

}