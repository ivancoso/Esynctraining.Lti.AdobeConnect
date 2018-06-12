using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Mvc;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("meetings")]
    public class MeetingsController : BaseApiController
    {
        private readonly IJsonSerializer _jsonSerializer;

        //private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        //private readonly LmsUserSessionModel _userSessionModel;
        private readonly ZoomUserService _userService;

        private readonly ZoomMeetingService _meetingService;
        //private readonly LmsFactory _lmsFactory;

        #region Constructors and Destructors

        public MeetingsController(
            //MeetingSetup meetingSetup,
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, IJsonSerializer jsonSerializer,
            ZoomUserService userService, ZoomRecordingService recordingService, ZoomMeetingService meetingService)
            : base(settings, logger)
        {
            //_meetingSetup = meetingSetup;
            _jsonSerializer = jsonSerializer;
            //_userSessionModel = userSessionModel;
            _userService = userService;
            _meetingService = meetingService;
        }

        #endregion

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<MeetingViewModel>>> GetCourseMeetings()
        {
            StringBuilder trace = null;

            string userId = null;
            try
            {
                var user = _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
            }
            //if (!string.IsNullOrEmpty(userId))
            //{

            var zoomMeetings = await _meetingService.GetMeetings(LmsLicense.Id, CourseId, userId);
            return zoomMeetings.ToSuccessResult();
            //}

            //return OperationResultWithData<IEnumerable<MeetingViewModel>>.Error("User does not exist in Zoom.");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingDetailsViewModel>> GetMeetingDetails(int meetingId)
        {
            StringBuilder trace = null;

            try
            {
                var viewModel = await _meetingService.GetMeetingDetails(meetingId, LmsLicense.Id, CourseId.ToString());
                return viewModel.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Details error.", e);
            }
            return OperationResultWithData<MeetingDetailsViewModel>.Error("Unexpected error happened");
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingViewModel>> Create(
            [FromBody] CreateMeetingViewModel requestDto)
        {
            OperationResultWithData<MeetingViewModel> result = null;
            try
            {
                string userId = null;
                try
                {
                    var user = _userService.GetUser(Param.lis_person_contact_email_primary);
                    userId = user.Id;
                }
                catch (Exception e)
                {
                    Logger.Error("User doesn't exist or doesn't belong to this account", e);
                    /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                    var userInfo = _userService.CreateUser(new CreateUserDto
                    {
                        Email = Param.lis_person_contact_email_primary,
                        FirstName = Param.PersonNameGiven,
                        LastName = Param.PersonNameFamily
                    });

                    return OperationResultWithData<MeetingViewModel>.Error(
                        "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    var createResult = await _meetingService.CreateMeeting(LmsLicense.Id, CourseId.ToString(),
                        userId,
                        Param.lis_person_contact_email_primary, requestDto);

                    return createResult;
                }

                return OperationResultWithData<MeetingViewModel>.Error("User does not exist in Zoom.");
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateMeeting", ex);
                return OperationResultWithData<MeetingViewModel>.Error(errorMessage);
            }

        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> Update([FromBody] CreateMeetingViewModel vm,
            [FromRoute] int meetingId)
        {
            string userId = null;
            try
            {
                var updated = await _meetingService.UpdateMeeting(meetingId, LmsLicense.Id, CourseId.ToString(),
                    Param.lis_person_contact_email_primary, vm);

                return updated ? OperationResult.Success() : OperationResult.Error("Meeting has not been updated");
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Update error.", e);
            }
            return OperationResult.Error("Unexpected error happened");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}/join")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        //[LmsAuthorizeBase]
        public virtual async Task<ActionResult> JoinMeeting(int meetingId, string session)
        {
            //            LmsCompany credentials = null;
            //            string sessionId = null;

            //                var s = GetReadOnlySession(session);
            //                sessionId = s.Id.ToString();
            //                credentials = s.LmsCompany;
            //                var param = s.LtiSession.LtiParam;
            //                var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, meetingId);

            //            //if (meeting == null && credentials.ConsumerKey == "b622bf8b-a120-4b40-816e-05f530a750d9" && param.course_id == 557)
            //            //{
            //            //    var mId = meetingId / 100000;
            //            //    viewModel = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, mId);
            //            if (dbMeeting == null)
            //                    //404
            //                    return NotFound(meetingId);

            //                string userId;
            //            try
            //                {
            //                    /*
            //                     {
            //    "code": 1010,
            //    "message": "User not belong to this account"
            //}*/
            //                    var user = _userService.GetUser(param.lis_person_contact_email_primary);
            //                    userId = user.Id;
            //                }
            //                catch (Exception e)
            //                {
            //                    Logger.Error("User doesn't exist or doesn't belong to this account", e);
            //                    /*{
            //"code": 1005,
            //"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
            //}*/
            //                    var userInfo = _userService.CreateUser(new CreateUserDto
            //                    {
            //                        Email = param.lis_person_contact_email_primary,
            //                        FirstName = param.PersonNameGiven,
            //                        LastName = param.PersonNameFamily
            //                    });

            //                    return Content(
            //                        "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
            //                }
            //                if (!string.IsNullOrEmpty(userId))
            //                {
            //                    var url = await _meetingService.GetMeetingUrl(userId, dbMeeting.ScoId, param.lis_person_contact_email_primary,
            //                        async () =>
            //                        {
            //                            var lmsService = _lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
            //                            var lmsUsers = await lmsService.GetUsers(credentials, CourseId);
            //                            var registrant = lmsUsers.Data.FirstOrDefault(x =>
            //                                !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(param.lis_person_contact_email_primary));
            //                            var registrantDto =
            //                                new RegistrantDto
            //                                {
            //                                    Email = registrant?.Email,
            //                                    FirstName = registrant?.GetFirstName(),
            //                                    LastName = registrant?.GetLastName()
            //                                };
            //                            return registrantDto;
            //                        });
            //                    return Redirect(url);
            //                }

            return Content("Error when joining.");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteMeeting(int meetingId, [FromQuery] bool remove = false)
        {
            var param = Session;
            //param.lis_person_contact_email_primary


            return OperationResult.Success();

            //return OperationResult.Error("Error during delete. Please try again or contact support.");
        }

        //protected LmsUserSession GetReadOnlySession(string key)
        //{
        //    Guid uid;
        //    var session = Guid.TryParse(key, out uid) ? this._userSessionModel.GetByIdWithRelated(uid).Value : null;

        //    if (session == null)
        //    {
        //        Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
        //        throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
        //    }

        //    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

        //    return session;
        //}
    }
}