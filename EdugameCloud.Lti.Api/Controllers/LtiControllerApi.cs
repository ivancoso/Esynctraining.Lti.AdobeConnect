﻿namespace EdugameCloud.Lti.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
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
    using Microsoft.AspNetCore.Mvc;

    [Route("")]
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

        private LmsCompanyModel LmsCompanyModel => IoC.Resolve<LmsCompanyModel>();

        #region Constructors and Destructors

        //public LtiApiController(
        //    LmsUserSessionModel userSessionModel,
        //    LmsUserModel lmsUserModel, 
        //    MeetingSetup meetingSetup, 
        //    ApplicationSettingsProvider settings, 
        //    UsersSetup usersSetup,
        //    IAdobeConnectAccountService acAccountService,
        //    ILogger logger,
        //    ICache cache) :base(userSessionModel, acAccountService, settings, logger, cache)
        //{
        //    this.lmsUserModel = lmsUserModel;
        //    this.meetingSetup = meetingSetup;
        //    this.usersSetup = usersSetup;
        //}

        #endregion

        #region Public Methods and Operators

        [Route("settings/save")]
        [HttpPost]
        public virtual OperationResultWithData<LmsUserSettingsDTO> SaveSettings([FromBody]LmsUserSettingsDTO settings)
        {
            try
            {
                var lmsProviderName = settings.lmsProviderName;
                var param = Session.LtiSession.LtiParam;
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, LmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    lmsUser = Session.LmsUser ?? new LmsUser
                    {
                        LmsCompany = LmsCompanyModel.GetOneById(LmsCompany.Id).Value,
                        UserId = param.lms_user_id,
                        Username = param.GetUserNameOrEmail(),
                    };
                }

                var acConnectionMode = (AcConnectionMode)settings.acConnectionMode;
                lmsUser.PrimaryColor = settings.primaryColor;

                if (acConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
                {
                    var provider = GetAdminProvider();
                    var couldSavePassword = usersSetup.SetACPassword(provider, LmsCompany, lmsUser, param, settings.password);
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
                string errorMessage = GetOutputErrorMessage("SaveSettings", ex);
                return OperationResultWithData<LmsUserSettingsDTO>.Error(errorMessage);
            }
        }
        
        [Route("settings/checkpass")]
        [HttpPost]
        public virtual OperationResultWithData<bool> CheckPasswordBeforeJoin([FromBody]RequestDto request)
        {
            try
            {
                bool isValid = false;
                var param = Session.LtiSession.With(x => x.LtiParam);
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, LmsCompany.Id).Value;
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
                string errorMessage = GetOutputErrorMessage("CheckPasswordBeforeJoin", ex);
                return OperationResultWithData<bool>.Error(errorMessage);
            }
        }

        [Route("users")]
        [HttpPost]
        public virtual OperationResultWithData<IList<LmsUserDTO>> GetUsers([FromBody]MeetingRequestDtoEx request)
        {
            try
            {
                var service = LmsFactory.GetUserService((LmsProviderEnum)LmsCompany.LmsProviderId);

                if (request.forceUpdate && LmsCompany.UseSynchronizedUsers
                    && service != null
                    && service.CanRetrieveUsersFromApiForCompany(LmsCompany)
                    && LmsCompany.LmsCourseMeetings != null)
                {
                    SynchronizationUserService.SynchronizeUsers(LmsCompany, syncACUsers: false, meetingIds: new[] { request.meetingId });
                }

                string error;
                IList<LmsUserDTO> users = this.usersSetup.GetUsers(
                    LmsCompany,
                    GetAdminProvider(),
                    Session?.LtiSession?.LtiParam,
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
                string errorMessage = GetOutputErrorMessage("GetUsers", ex);
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
        [Route("meeting/leave")]
        [HttpPost]
        public virtual OperationResult LeaveMeeting([FromBody]MeetingRequestDto request)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                OperationResult result = this.meetingSetup.LeaveMeeting(LmsCompany, param, request.meetingId, GetAdminProvider());
                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("LeaveMeeting", ex);
                return OperationResult.Error(errorMessage);
            }
        }
        
        [Route("meeting/SetDefaultACRoles")]
        [HttpPost]
        public virtual OperationResultWithData<List<LmsUserDTO>> SetDefaultRolesForNonParticipants([FromBody]MeetingRequestDto request)
        {
            try
            {
                var param = Session.LtiSession.With(x => x.LtiParam);
                string error = null;
                List<LmsUserDTO> updatedUsers = this.usersSetup.SetDefaultRolesForNonParticipants(
                    LmsCompany,
                    this.GetAdminProvider(),
                    param,
                    request.meetingId,
                    out error);

                //if (string.IsNullOrEmpty(error))
                    return OperationResultWithData<List<LmsUserDTO>>.Success(error, updatedUsers);

                //return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SetDefaultRolesForNonParticipants", ex);
                return OperationResultWithData<List<LmsUserDTO>>.Error(errorMessage);
            }
        }

        #endregion

    }

}