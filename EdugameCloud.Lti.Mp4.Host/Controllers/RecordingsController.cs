using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EdugameCloud.Lti.Api.Controllers;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Mp4.Host.Dto;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    [Route("recordings")]
    [LmsAuthorizeBase]
    public class RecordingsController : BaseApiController
    {
        private static readonly MapperConfiguration mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<RecordingDto, RecordingWithMp4Dto>());
        private static readonly MapperConfiguration seminarMapConfig = new MapperConfiguration(cfg => cfg.CreateMap<SeminarSessionRecordingDto, SeminarRecordingWithMp4Dto>());
        private readonly IMp4LinkBuilder _mp4LinkBuilder;
        private readonly IVttLinkBuilder _vttLinkBuilder;

        private IRecordingsService RecordingsService { get; }


        public RecordingsController(
            IMp4LinkBuilder mp4LinkBuilder,
            IVttLinkBuilder vttLinkBuilder,
            IRecordingsService recordingsService,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,             
            ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            _mp4LinkBuilder = mp4LinkBuilder;
            _vttLinkBuilder = vttLinkBuilder;
            RecordingsService = recordingsService;
        }


        [Route("")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<IEnumerable<IMp4StatusContainer>>> GetAllMeetingRecordings([FromBody]RecordingsRequestDto input)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var ac = GetAdminProvider();

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)input.LmsMeetingType, IoC.Resolve<API.AdobeConnect.ISeminarService>());

                IEnumerable<IRecordingDto> rawRecordings = RecordingsService.GetRecordings(
                    LmsCompany,
                    ac,
                    Session.LmsCourseId,
                    input.MeetingId,
                    getRoomTypeFactory);

                var smap = seminarMapConfig.CreateMapper();
                var map = mapConfig.CreateMapper();

                IEnumerable<RecordingWithMp4Dto> recordings =
                    ((LmsMeetingType)input.LmsMeetingType == LmsMeetingType.Seminar)
                    ? rawRecordings.Select(x => smap.Map<SeminarRecordingWithMp4Dto>(x))
                    : rawRecordings.Select(x => map.Map<RecordingWithMp4Dto>(x));

                if (!new LmsRoleService(Settings).IsTeacher(param, LmsCompany) && !LmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.Published).ToList();
                }
                Guid mp4;
                if (!Guid.TryParse(LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey), out mp4))
                    mp4 = Guid.Empty;
                Guid mp4Subtitles;
                if (!Guid.TryParse(LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey), out mp4Subtitles))
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
                string errorMessage = GetOutputErrorMessage("MP4-GetAllMeetingRecordings", ex);
                return OperationResultWithData<IEnumerable<IMp4StatusContainer>>.Error(errorMessage);
            }
        }

        [Route("status")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<Mp4TaskStatusDto>> GetRecordingStatus([FromBody]RecordingActionRequestDto input)
        {
            try
            {
                Guid mp4;
                if (!Guid.TryParse(LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey), out mp4))
                    mp4 = Guid.Empty;
                Guid mp4Subtitles;
                if (!Guid.TryParse(LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey), out mp4Subtitles))
                    mp4Subtitles = Guid.Empty;

                if ((mp4 != Guid.Empty) || (mp4Subtitles != Guid.Empty))
                {
                    var mp4Client = IoC.Resolve<TaskClient>();
                    return await Mp4ApiUtility.GetRecordingStatus(mp4Client, input.RecordingId,
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
                string errorMessage = GetOutputErrorMessage("MP4-GetRecordings", ex);
                return OperationResultWithData<Mp4TaskStatusDto>.Error(errorMessage);
            }
        }

    }

}
