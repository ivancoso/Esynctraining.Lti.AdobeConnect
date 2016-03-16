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


        [Route("all")]
        [HttpPost]
        public virtual async Task<OperationResultWithData<IEnumerable<RecordingWithMp4Dto>>> GetAllMeetingRecordings(RecordingsRequestDto input)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(input.LmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;

                IEnumerable<RecordingDTO> rawRecordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param.course_id,
                    input.MeetingId);

                var mapper = mapConfig.CreateMapper();
                List<RecordingWithMp4Dto> recordings = rawRecordings.Select(x => mapper.Map<RecordingWithMp4Dto>(x)).ToList();

                if (!new LmsRoleService(Settings).IsTeacher(param) && !lmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.published).ToList();
                }
                
                string mp4LicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                string mp4WithSubtitlesLicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (!string.IsNullOrWhiteSpace(mp4LicenseKey) || !string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                {
                    var mp4Tasks = new Dictionary<string, MP4Service.Contract.Client.DataTask>();
                    foreach (var recordingScoId in recordings.Where(x => !x.is_mp4).Select(x => x.id))
                    {
                        mp4Tasks.Add(recordingScoId, new MP4Service.Contract.Client.DataTask());
                    }

                    var mp4 = new ConcurrentDictionary<string, MP4Service.Contract.Client.DataTask>(mp4Tasks);

                    if (!string.IsNullOrWhiteSpace(mp4LicenseKey))
                    {
                        //foreach (var recording in mp4)
                        //Parallel.ForEach(mp4, (recording) =>
                        //{
                        //    var mp4Client = IoC.Resolve<TaskClient>();
                        //    Mp4ApiUtility.CheckStatus(mp4Client, Guid.Parse(mp4LicenseKey), long.Parse(recording.Key), recording.Value, logger);

                        //});
                        await Task.Run(() => Parallel.ForEach(mp4, (recording) =>
                        {
                            var mp4Client = IoC.Resolve<TaskClient>();
                            Mp4ApiUtility.CheckStatus(mp4Client, Guid.Parse(mp4LicenseKey), long.Parse(recording.Key), recording.Value, logger);

                        })).ConfigureAwait(false);
                    }

                    if (!string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                    {
                        //foreach (var recording in mp4)
                        //Parallel.ForEach(mp4, (recording) =>
                        //{
                        //    var mp4Client = IoC.Resolve<TaskClient>();
                        //    Mp4ApiUtility.CheckStatus(mp4Client, Guid.Parse(mp4WithSubtitlesLicenseKey), long.Parse(recording.Key), recording.Value, logger);

                        //});

                        await Task.Run(() => Parallel.ForEach(mp4, (recording) =>
                        {
                            var mp4Client = IoC.Resolve<TaskClient>();
                            Mp4ApiUtility.CheckStatus(mp4Client, Guid.Parse(mp4WithSubtitlesLicenseKey), long.Parse(recording.Key), recording.Value, logger);

                        })).ConfigureAwait(false);
                    }

                    foreach (var item in mp4)
                    {
                        if (item.Value.Duration == -777)
                        {
                            recordings.FirstOrDefault(x => x.id == item.Key).Mp4 = new Mp4TaskStatusDto
                            {
                                status = "MP4 Service Error",
                            };
                            continue;
                        }

                        if (string.IsNullOrEmpty(item.Value.ScoId))
                            continue;

                        var recording = recordings.FirstOrDefault(x => x.id == item.Key);
                        recording.Mp4 = new Mp4TaskStatusDto
                        {
                            mp4_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Uploaded) ? item.Value.UploadScoId : null,
                            cc_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Transcripted) ? item.Value.TranscriptScoId : null,
                            status = item.Value.Status.ToString(),
                        };
                    }

                }

                return OperationResultWithData<IEnumerable<RecordingWithMp4Dto>>.Success(recordings);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("MP4-GetRecordings", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<RecordingWithMp4Dto>>.Error(errorMessage);
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
