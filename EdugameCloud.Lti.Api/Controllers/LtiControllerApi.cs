namespace EdugameCloud.Lti.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EdugameCloud.Lti.Api.Filters;
    using EdugameCloud.Lti.Api.Models;
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
        #region Fields

        private readonly LmsUserModel lmsUserModel;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;

        #endregion

        private LmsCompanyModel LmsCompanyModel => IoC.Resolve<LmsCompanyModel>();

        #region Constructors and Destructors

        public LtiApiController(
            LmsUserModel lmsUserModel,
            MeetingSetup meetingSetup,
            ApplicationSettingsProvider settings,
            UsersSetup usersSetup,
            IAdobeConnectAccountService acAccountService,
            ILogger logger,
            ICache cache) :base(acAccountService, settings, logger, cache)
        {
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.usersSetup = usersSetup;
        }

        #endregion

        #region Public Methods and Operators

        [Route("settings/save")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<LmsUserSettingsDTO> SaveSettings([FromBody]LmsUserSettingsDTO settings)
        {
            try
            {
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

                var acConnectionMode = (AcConnectionMode)settings.AcConnectionMode;
                lmsUser.PrimaryColor = settings.PrimaryColor;

                if (acConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
                {
                    var provider = GetAdminProvider();
                    var couldSavePassword = usersSetup.SetACPassword(provider, LmsCompany, lmsUser, param, settings.Password);
                    if (!couldSavePassword)
                    {
                        return OperationResultWithData<LmsUserSettingsDTO>.Error(EdugameCloud.Lti.Resources.Messages.IncorrectAcPassword);
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
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<bool> CheckPasswordBeforeJoin()
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

        //[Route("users")]
        //[HttpPost]
        //public virtual OperationResultWithData<IList<LmsUserDTO>> GetUsers([FromBody]MeetingRequestDtoEx request)
        //{
        //    try
        //    {
        //        var service = LmsFactory.GetUserService((LmsProviderEnum)LmsCompany.LmsProviderId);

        //        if (request.ForceUpdate && LmsCompany.UseSynchronizedUsers
        //            && service != null
        //            && LmsCompany.LmsCourseMeetings != null)
        //        {
        //            SynchronizationUserService.SynchronizeUsers(LmsCompany, syncACUsers: false, meetingIds: new[] { request.meetingId });
        //        }

        //        string error;
        //        IList<LmsUserDTO> users = this.usersSetup.GetUsers(
        //            LmsCompany,
        //            GetAdminProvider(),
        //            Session?.LtiSession?.LtiParam,
        //            request.meetingId,
        //            out error,
        //            null);

        //        if (string.IsNullOrWhiteSpace(error))
        //        {
        //            return users.ToSuccessResult();
        //        }

        //        return OperationResultWithData<IList<LmsUserDTO>>.Error(error);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetUsers", ex);
        //        return OperationResultWithData<IList<LmsUserDTO>>.Error(errorMessage);
        //    }
        //}
        
        [Route("meeting/leave")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResult LeaveMeeting([FromBody]MeetingRequestDto request)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                OperationResult result = this.meetingSetup.LeaveMeeting(LmsCompany, param, request.MeetingId, GetAdminProvider());
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
        [LmsAuthorizeBase]
        public virtual async Task<OperationResultWithData<List<LmsUserDTO>>> SetDefaultRolesForNonParticipants([FromBody]MeetingRequestDto request)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var updatedUsers = await usersSetup.SetDefaultRolesForNonParticipants(
                    LmsCompany,
                    GetAdminProvider(),
                    param,
                    request.MeetingId);

                return OperationResultWithData<List<LmsUserDTO>>.Success(updatedUsers.Item2, updatedUsers.Item1);
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