using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Resources;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("seminars")]
    public class SeminarController : BaseApiController
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeetingDTO, SeminarDto>()).CreateMapper();
        private readonly API.AdobeConnect.ISeminarService _seminarService;

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();
        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();


        public SeminarController(
            API.AdobeConnect.ISeminarService seminarService,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache
        )
            : base(acAccountService, settings, logger, cache)
        {
            _seminarService = seminarService;
        }

        [Route("create")]
        [HttpPost]
        [LmsAuthorizeBase(FeatureName = LmsLicenseSettingNames.SeminarsEnable)]
        public virtual async Task<OperationResult> Create([FromBody]CreateSeminarDto model)
        {
            if (string.IsNullOrWhiteSpace(model.SeminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", nameof(model.SeminarLicenseId));

            //TRICK:
            model.Type = (int)LmsMeetingType.Seminar;

            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();
                var fb = new SeminarFolderBuilder(model.SeminarLicenseId);

                OperationResult ret = await MeetingSetup.SaveMeeting(
                    LmsCompany,
                    GetUserProvider(),
                    param,
                    model,
                    trace,
                    fb);

                return TrickForSeminar(ret, model.SeminarLicenseId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Create", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("edit")]
        [HttpPost]
        [LmsAuthorizeBase(FeatureName = LmsLicenseSettingNames.SeminarsEnable)]
        public virtual async Task<OperationResult> Edit([FromBody]EditSeminarDto model)
        {
            if (string.IsNullOrWhiteSpace(model.SeminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", nameof(model.SeminarLicenseId));
        
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();
                var fb = new SeminarFolderBuilder(model.SeminarLicenseId);
                OperationResult ret = await MeetingSetup.SaveMeeting(
                    LmsCompany,
                    GetUserProvider(),
                    param,
                    model,
                    trace,
                    fb);

                return TrickForSeminar(ret, model.SeminarLicenseId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Edit", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("sessions/create")]
        [Route("sessions/edit")]
        [HttpPost]
        [LmsAuthorizeBase(FeatureName = LmsLicenseSettingNames.SeminarsEnable)]
        public OperationResult SaveSeminarSession([FromBody]SeminarSessionInputDto seminarSessionDto)
        {
            try
            {
                // TRICK: change record meeting id to meeting sco-id
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, long.Parse(seminarSessionDto.SeminarRoomId));
                if (meeting == null)
                {
                    return OperationResult.Error(Messages.MeetingNotFound);
                }

                //NOTE: save under admin account doesn't work for user license
                var ac = GetUserProvider();
                ProcessQuota(ac, meeting.ScoId, seminarSessionDto);

                //var timeZone = AcAccountService.GetAccountDetails(ac, IoC.Resolve<ICache>()).TimeZoneInfo;
                var meetingUpdateResult = _seminarService.SaveSeminarSession(seminarSessionDto, meeting.ScoId, ac/*, timeZone*/);
                return meetingUpdateResult;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSeminarSession", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("sessions/delete")]
        [HttpPost]
        [LmsAuthorizeBase(FeatureName = LmsLicenseSettingNames.SeminarsEnable)]
        public OperationResult DeleteSeminarSession([FromBody]DeleteSeminarSessionDto model)
        {
            if (string.IsNullOrWhiteSpace(model.SeminarSessionId))
                throw new ArgumentException("seminarSessionId can't be empty", nameof(model.SeminarSessionId));

            //TODO: CHECK permission for deletion
            //TODO: to service
            try
            {
                var ac = GetUserProvider();
                var result = ac.DeleteSco(model.SeminarSessionId);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSeminarSession", ex);
                return OperationResult.Error(errorMessage);
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
        
        private OperationResult TrickForSeminar(OperationResult ret, string seminarLicenseId)
        {
            var res1 = ret as OperationResultWithData<MeetingDTO>;
            if (res1 != null)
            {
                var resultDto = mapper.Map<SeminarDto>(res1.Data);
                resultDto.SeminarLicenseId = seminarLicenseId;
                return resultDto.ToSuccessResult();
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
                return result.ToSuccessResult();
            }

            return ret;
        }

    }

}
