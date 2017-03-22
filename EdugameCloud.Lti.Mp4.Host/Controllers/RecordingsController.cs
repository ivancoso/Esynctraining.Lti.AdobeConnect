using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Mp4.Host.Dto;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    [RoutePrefix("recordings")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RecordingsController : BaseController
    {
        private static readonly MapperConfiguration mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<RecordingDto, RecordingWithMp4Dto>());
        private static readonly MapperConfiguration seminarMapConfig = new MapperConfiguration(cfg => cfg.CreateMap<SeminarSessionRecordingDto, SeminarRecordingWithMp4Dto>());
        private readonly IMp4LinkBuilder _mp4LinkBuilder;
        private readonly IVttLinkBuilder _vttLinkBuilder;

        private IRecordingsService RecordingsService => IoC.Resolve<IRecordingsService>();


        public RecordingsController(
            LmsUserSessionModel userSessionModel,
            Esynctraining.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, 
            IMp4LinkBuilder mp4LinkBuilder, 
            IVttLinkBuilder vttLinkBuilder)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            _mp4LinkBuilder = mp4LinkBuilder;
            _vttLinkBuilder = vttLinkBuilder;
        }


        [Route("")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<IEnumerable<IMp4StatusContainer>>> GetAllMeetingRecordings(RecordingsRequestDto input)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var ac = GetAdminProvider(lmsCompany);

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)input.LmsMeetingType, IoC.Resolve<API.AdobeConnect.ISeminarService>());

                IEnumerable<IRecordingDto> rawRecordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    ac,
                    param.course_id,
                    input.MeetingId,
                    getRoomTypeFactory);

                var smap = seminarMapConfig.CreateMapper();
                var map = mapConfig.CreateMapper();

                IEnumerable<RecordingWithMp4Dto> recordings =
                    ((LmsMeetingType)input.LmsMeetingType == LmsMeetingType.Seminar)
                    ? rawRecordings.Select(x => smap.Map<SeminarRecordingWithMp4Dto>(x))
                    : rawRecordings.Select(x => map.Map<RecordingWithMp4Dto>(x));

                if (!new LmsRoleService(Settings).IsTeacher(param) && !lmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.Published).ToList();
                }
                Guid mp4;
                if (!Guid.TryParse(lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey), out mp4))
                    mp4 = Guid.Empty;
                Guid mp4Subtitles;
                if (!Guid.TryParse(lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey), out mp4Subtitles))
                    mp4Subtitles = Guid.Empty;

                IEnumerable<IMp4StatusContainer> result = await Mp4ApiUtility.ProcessMp4(recordings.Cast<IMp4StatusContainer>().ToList(),
                    mp4,
                    mp4Subtitles,
                    _mp4LinkBuilder,
                    _vttLinkBuilder,
                    Logger);

                return result.ToSuccessResult();
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
                var session = Session;
                lmsCompany = session.LmsCompany;

                Guid mp4;
                if (!Guid.TryParse(lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey), out mp4))
                    mp4 = Guid.Empty;
                Guid mp4Subtitles;
                if (!Guid.TryParse(lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey), out mp4Subtitles))
                    mp4Subtitles = Guid.Empty;

                if ((mp4 != Guid.Empty) || (mp4Subtitles != Guid.Empty))
                {
                    var mp4Client = IoC.Resolve<TaskClient>();
                    return await Mp4ApiUtility.GetRecordingStatus(mp4Client, input.RecordingId.ToString(),
                        mp4,
                        mp4Subtitles,
                        _mp4LinkBuilder,
                        _vttLinkBuilder,
                        Logger);
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
