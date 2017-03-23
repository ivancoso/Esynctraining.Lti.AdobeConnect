﻿using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Caching;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using EdugameCloud.Core.Business;
using System.Runtime.Caching;
using Esynctraining.AdobeConnect;
using EdugameCloud.Lti.Core.DTO;
using AutoMapper;

namespace EdugameCloud.Lti.Controllers
{
    public class SeminarController : BaseController
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeetingDTO, SeminarDto>()).CreateMapper();

        //// TRICK:
        //public class SeminarMeetingDTO : MeetingDTO
        //{
        //}


        protected readonly ObjectCache _cache = MemoryCache.Default;
        private readonly API.AdobeConnect.ISeminarService _seminarService;


        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private API.AdobeConnect.IAdobeConnectAccountService AcAccountService => IoC.Resolve<API.AdobeConnect.IAdobeConnectAccountService>();

        #region Constructors and Destructors

        public SeminarController(
            API.AdobeConnect.ISeminarService seminarService,
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            _seminarService = seminarService;
        }

        #endregion

        //[HttpPost]
        //public virtual JsonResult GetAll(string lmsProviderName)
        //{
        //    try
        //    {
        //        var session = GetSession(lmsProviderName);

        //        if (session.LmsUser == null)
        //            return Json(OperationResult.Error("Session doesn't contain LMS user."));
        //        if (string.IsNullOrWhiteSpace(session.LmsUser.PrincipalId))
        //            return Json(OperationResult.Error("You don't have Adobe Connect account."));

        //        var provider = GetAdobeConnectProvider(session.LmsCompany);

        //        var seminars = _seminarService.GetLicensesWithContent(provider, session.LmsUser, session.LtiSession.LtiParam, session.LmsCompany);

        //        return Json(OperationResultWithData<IEnumerable<SeminarLicenseDto>>.Success(seminars));
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("SeminarController.GetAll", ex);
        //        return Json(OperationResult.Error(errorMessage));
        //    }

        //}

        [HttpPost]
        public JsonResult SaveSeminarSession(string lmsProviderName, SeminarSessionInputDto seminarSessionDto)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (seminarSessionDto == null)
                throw new ArgumentNullException(nameof(seminarSessionDto));

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                //save under admin account doesn't work for user license
                var ac = GetCurrentUserProvider(session);

                // TRICK: change record meeting id to meeting sco-id
                var param = session.LtiSession.With(x => x.LtiParam);
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, long.Parse(seminarSessionDto.SeminarRoomId));
                if (meeting == null)
                {
                    return Json(OperationResult.Error(Resources.Messages.MeetingNotFound));
                }
                
                ProcessQuota(ac, meeting.ScoId, seminarSessionDto);

                var timeZone = AcAccountService.GetAccountDetails(ac, IoC.Resolve<ICache>()).TimeZoneInfo;
                var meetingUpdateResult = _seminarService.SaveSeminarSession(seminarSessionDto, meeting.ScoId, ac, timeZone);
                return Json(meetingUpdateResult);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSeminarSession", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        private void ProcessQuota(IAdobeConnectProxy ac, string meetingScoId, SeminarSessionInputDto seminarSessionDto)
        {
            var seminar = ac.GetScoInfo(meetingScoId).ScoInfo;
            var license = _seminarService.GetSharedSeminarLicenses(ac).FirstOrDefault(x => x.ScoId == seminar.FolderId);
            if (license != null && !license.IsExpired)
            {
                seminarSessionDto.ExpectedLoad = license.Quota.Value;
                return;
            }

            var userLicense = _seminarService.GetUserSeminarLicenses(ac).FirstOrDefault(x => x.ScoId == seminar.FolderId);

            if (userLicense == null)
                throw new InvalidOperationException($"Not found seminar license for seminar '{seminar.Name}'({seminar.ScoId}).");
            seminarSessionDto.ExpectedLoad = userLicense.LicenseQuota;
        }

        [HttpPost]
        public JsonResult DeleteSeminarSession(string lmsProviderName, string seminarSessionId)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("seminarSessionId can't be empty", nameof(seminarSessionId));

            //TODO: CHECK permission for deletion
            //TODO: to service
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var ac = GetCurrentUserProvider(session);

                var result = ac.DeleteSco(seminarSessionId);
                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSeminarSession", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }


        private IAdobeConnectProxy GetCurrentUserProvider(LmsUserSession session)
        {
            string cacheKey = CachePolicies.Keys.UserAdobeConnectProxy(session.LmsCompany.Id, session.LtiSession.LtiParam.lms_user_id);
            var provider = _cache.Get(cacheKey) as IAdobeConnectProxy;

            if (provider == null)
            {
                string breezeSession = LoginCurrentUser(session);
                var accService = new Esynctraining.AdobeConnect.AdobeConnectAccountService(Logger);
                provider = accService.GetProvider2(new AdobeConnectAccess2(new Uri(session.LmsCompany.AcServer), breezeSession));

                var sessionTimeout = accService.GetAccountDetails(GetAdminProvider(session.LmsCompany)).SessionTimeout - 1; //-1 is to be sure 
                _cache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        private JsonResult TrickForSeminar(OperationResult ret, string seminarLicenseId)
        {
            var res1 = ret as OperationResultWithData<MeetingDTO>;
            if (res1 != null)
            {
                var resultDto = mapper.Map<SeminarDto>(res1.Data);
                resultDto.SeminarLicenseId = seminarLicenseId;
                return Json(resultDto.ToSuccessResult());
            }


            var res2 = ret as OperationResultWithData<MeetingAndLmsUsersDTO>;
            if (res2 != null)
            {
                var seminarDto = mapper.Map<SeminarDto>(res2.Data.Meeting);
                var result = new SeminarAndLmsUsersDTO
                {
                    Meeting = seminarDto,
                    LmsUsers = res2.Data.LmsUsers,
                };
                return Json(result.ToSuccessResult());
            }

            return Json(ret);
        }

        private string LoginCurrentUser(LmsUserSession session)
        {
            LmsCompany lmsCompany = null;
            try
            {
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var LmsUserModel = IoC.Resolve<LmsUserModel>();
                var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                if (lmsUser.PrincipalId == null)
                {
                    throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                var ac = GetAdminProvider(session.LmsCompany);
                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                var MeetingSetup = IoC.Resolve<MeetingSetup>();
                string breezeToken = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, ac);

                return breezeToken;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", lmsCompany, ex);
                throw;
            }
        }

    }

}