using System;
using System.Collections.Generic;
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
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Lti.Controllers
{
    public class SeminarController : BaseController
    {
        private readonly ISeminarService _seminarService;


        private MeetingSetup MeetingSetup
        {
            get { return IoC.Resolve<MeetingSetup>(); }
        }

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        private IAdobeConnectAccountService AcAccountService
        {
            get { return IoC.Resolve<IAdobeConnectAccountService>(); }
        }

        #region Constructors and Destructors

        public SeminarController(
            ISeminarService seminarService,
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService, 
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
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
        public virtual JsonResult Create(string lmsProviderName, string seminarLicenseId,  MeetingDTO meeting)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (string.IsNullOrWhiteSpace(seminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", "seminarLicenseId");
            if (meeting == null)
                throw new ArgumentNullException("meeting");

            //TRICK:
            meeting.type = (int)LmsMeetingType.Seminar;

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var trace = new StringBuilder();

                var fb = new SeminarFolderBuilder(seminarLicenseId);

                OperationResult ret = MeetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting,
                    trace,
                    fb);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Create", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult Edit(string lmsProviderName, string seminarLicenseId, MeetingDTO meeting)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (string.IsNullOrWhiteSpace(seminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", "seminarLicenseId");
            if (meeting == null)
                throw new ArgumentNullException("meeting");

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var trace = new StringBuilder();
                var fb = new SeminarFolderBuilder(seminarLicenseId);
                OperationResult ret = MeetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting,
                    trace,
                    fb);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Edit", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public JsonResult SaveSeminarSession(string lmsProviderName, SeminarSessionDto seminarSessionDto)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (seminarSessionDto == null)
                throw new ArgumentNullException("seminarSessionDto");

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(credentials);

                // TRICK: change record meeting id to meeting sco-id
                var param = session.LtiSession.With(x => x.LtiParam);
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, long.Parse(seminarSessionDto.seminarRoomId));
                if (meeting == null)
                {
                    return Json(OperationResult.Error(Resources.Messages.MeetingNotFound));
                }

                seminarSessionDto.seminarRoomId = meeting.ScoId;
                ProcessQuota(ac, meeting.ScoId, seminarSessionDto);

                var timeZone = AcAccountService.GetAccountDetails(ac, IoC.Resolve<ICache>()).TimeZoneInfo;
                var meetingUpdateResult = _seminarService.SaveSeminarSession(seminarSessionDto, ac, timeZone);
                return Json(meetingUpdateResult);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSeminarSession", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        private void ProcessQuota(IAdobeConnectProxy ac, string meetingScoId, SeminarSessionDto seminarSessionDto)
        {
            var seminar = ac.GetScoInfo(meetingScoId).ScoInfo;
            var license = _seminarService.GetSharedSeminarLicenses(ac).FirstOrDefault(x => x.ScoId == seminar.FolderId);
            if (license != null)
            {
                if (license.QuotaId == "concurrent-user-per-meeting-quota")
                    seminarSessionDto.ExpectedLoad = license.Quota.Value;
                return;
            }

            // NOTE: it looks like we do not support users licenses for seminars.
            var userLicense = _seminarService.GetUserSeminarLicenses(ac).FirstOrDefault(x => x.ScoId == seminar.FolderId);

            if (userLicense == null)
                throw new InvalidOperationException($"Not found seminar license for seminar '{seminar.Name}'({seminar.ScoId}).");
            if (userLicense.QuotaId == "concurrent-user-per-meeting-quota")
                seminarSessionDto.ExpectedLoad = userLicense.Quota.Value;
        }

        [HttpPost]
        public JsonResult DeleteSeminarSession(string lmsProviderName, string seminarSessionId)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(lmsProviderName));
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                throw new ArgumentException("seminarSessionId can't be empty", "seminarSessionId");

            //TODO: CHECK permission for deletion
            //TODO: to service
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(credentials);

                var result = ac.DeleteSco(seminarSessionId);
                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSeminarSession", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}