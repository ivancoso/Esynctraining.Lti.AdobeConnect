using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Mp4.Host.Dto;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Esynctraining.WebApi.Client;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    [RoutePrefix("recordings")]
    [EnableCors(origins: "*", headers: "*", methods: "*")] //POST,OPTIONS
    public class RecordingsController : BaseController
    {
        private static readonly MapperConfiguration mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<RecordingDTO, RecordingWithMp4Dto>());
        private static readonly MapperConfiguration seminarMapConfig = new MapperConfiguration(cfg => cfg.CreateMap<SeminarSessionRecordingDto, SeminarRecordingWithMp4Dto>());


        private IRecordingsService RecordingsService
        {
            get { return IoC.Resolve<IRecordingsService>(); }
        }


        public RecordingsController(
            LmsUserSessionModel userSessionModel,
            Esynctraining.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
        }


        [Route("")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<IEnumerable<IMp4StatusContainer>>> GetAllMeetingRecordings(RecordingsRequestDto input)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(input.LmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var ac = this.GetAdobeConnectProvider(lmsCompany);

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)int.Parse(input.LmsMeetingType), IoC.Resolve<API.AdobeConnect.ISeminarService>());

                IEnumerable<IRecordingDto> rawRecordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    ac,
                    param.course_id,
                    input.MeetingId,
                    getRoomTypeFactory);

                var smap = seminarMapConfig.CreateMapper();
                var map = mapConfig.CreateMapper();

                IEnumerable<RecordingWithMp4Dto> recordings =
                    ((LmsMeetingType)int.Parse(input.LmsMeetingType) == LmsMeetingType.Seminar)
                    ? rawRecordings.Select(x => smap.Map<SeminarRecordingWithMp4Dto>(x))
                    : rawRecordings.Select(x => map.Map<RecordingWithMp4Dto>(x));

                if (!new LmsRoleService(Settings).IsTeacher(param) && !lmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.Published).ToList();
                }
                List<IMp4StatusContainer> 

                result = await Mp4ApiUtility.ProcessMp4(recordings.Cast<IMp4StatusContainer>().ToList(),
                    lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey),
                    lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey),
                    logger);

                return OperationResultWithData<IEnumerable<IMp4StatusContainer>>.Success(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("MP4-GetAllMeetingRecordings", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<IMp4StatusContainer>>.Error(errorMessage);
            }
        }

        [Route("status")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<Mp4TaskStatusDto>> GetRecordingStatus(RecordingActionRequestDto input)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(input.LmsProviderName);
                lmsCompany = session.LmsCompany;
                
                string mp4LicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                string mp4WithSubtitlesLicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (!string.IsNullOrWhiteSpace(mp4LicenseKey) || !string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                {
                    var mp4Client = IoC.Resolve<TaskClient>();
                    return await Mp4ApiUtility.GetRecordingStatus(mp4Client, input.RecordingId,
                        Guid.Parse(mp4LicenseKey),
                        Guid.Parse(mp4WithSubtitlesLicenseKey),
                        logger);
                }

                return OperationResultWithData<Mp4TaskStatusDto>.Error("No MP4 license found");
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("MP4-GetRecordings", lmsCompany, ex);
                return OperationResultWithData<Mp4TaskStatusDto>.Error(errorMessage);
            }
        }

    }

}
