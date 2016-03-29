using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Esynctraining.WebApi.Client;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public static class Mp4ApiUtility
    {
        public static async Task<OperationResult> DoConvert(TaskClient mp4Client,
            Guid licenseKey,
            MP4Service.Contract.Client.LicenseType expectedLicenseType,
            long recordingScoId,
            ILogger logger)
        {
            if (mp4Client == null)
                throw new ArgumentNullException("mp4Client");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (licenseKey == Guid.Empty)
                throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

            try
            {
                var license = await mp4Client.GetLicense(licenseKey).ConfigureAwait(false);
                if (license == null)
                    throw new WarningMessageException("MP4 license not found.");
                if (license.Type != expectedLicenseType)
                    throw new WarningMessageException("Invalid MP4 license type.");

                var task = await mp4Client.Convert(new MP4Service.Contract.Client.TaskParam
                {
                    LicenseId = licenseKey.ToString(),
                    ScoId = recordingScoId,
                }).ConfigureAwait(false);

                return OperationResultWithData<string>.Success(task.Status.ToString());
            }
            catch (AggregateException ex)
            {
                return ProcessAggregateException(ex, "Message from MP4 API: ", logger);
            }
            catch (ApiException ex)
            {
                return ProcessApiException(ex, "Message from MP4 API: ", logger);
            }
        }

        public static async Task<List<IMp4StatusContainer>> ProcessMp4(List<IMp4StatusContainer> recordings,
            string mp4LicenseKey,
            string mp4WithSubtitlesLicenseKey,
            ILogger logger)
        {
            if (recordings == null)
                throw new ArgumentNullException("recordings");
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (!string.IsNullOrWhiteSpace(mp4LicenseKey) || !string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
            {
                var mp4Tasks = new Dictionary<string, MP4Service.Contract.Client.DataTask>();
                foreach (var recordingScoId in recordings/*.Where(x => !x.is_mp4)*/.Select(x => x.Id))
                {
                    mp4Tasks.Add(recordingScoId, new MP4Service.Contract.Client.DataTask { Id = Guid.Empty });
                }

                var mp4 = new ConcurrentDictionary<string, MP4Service.Contract.Client.DataTask>(mp4Tasks);

                if (!string.IsNullOrWhiteSpace(mp4LicenseKey))
                {
                    await Task.Run(() => Parallel.ForEach(mp4, (recording) =>
                    {
                        var mp4Client = IoC.Resolve<TaskClient>();
                        CheckStatus(mp4Client, Guid.Parse(mp4LicenseKey), long.Parse(recording.Key), recording.Value, logger);

                    })).ConfigureAwait(false);
                }

                if (!string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                {
                    var nonProcessed = mp4.Where(x => x.Value.Id == Guid.Empty);
                    await Task.Run(() => Parallel.ForEach(nonProcessed, (recording) =>
                    {
                        var mp4Client = IoC.Resolve<TaskClient>();
                        CheckStatus(mp4Client, Guid.Parse(mp4WithSubtitlesLicenseKey), long.Parse(recording.Key), recording.Value, logger);

                    })).ConfigureAwait(false);
                }

                foreach (var item in mp4)
                {
                    if (item.Value.Duration == -777)
                    {
                        recordings.FirstOrDefault(x => x.Id == item.Key).Mp4 = new Mp4TaskStatusDto
                        {
                            status = "MP4 Service Error",
                        };
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.Value.ScoId))
                        continue;

                    var recording = recordings.FirstOrDefault(x => x.Id == item.Key);
                    recording.Mp4 = new Mp4TaskStatusDto
                    {
                        mp4_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Uploaded) ? item.Value.UploadScoId : null,
                        cc_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Transcripted) ? item.Value.TranscriptScoId : null,
                        status = item.Value.Status.ToString(),
                    };
                }

            }

            return recordings;
        }

        public static async Task<OperationResultWithData<Mp4TaskStatusDto>> GetRecordingStatus(TaskClient mp4Client,
            long recordingScoId,
            Guid licenseKey,
            Guid licenseKey2,
            ILogger logger)
        {
            if (mp4Client == null)
                throw new ArgumentNullException("mp4Client");
            if (string.IsNullOrWhiteSpace("recordingScoId"))
                throw new ArgumentException("recordingScoId can't be empty", "recordingScoId");
            if (logger == null)
                throw new ArgumentNullException("logger");

            string mp4LicenseKey = licenseKey.ToString();
            string mp4WithSubtitlesLicenseKey = licenseKey2.ToString();
            if (!string.IsNullOrWhiteSpace(mp4LicenseKey) || !string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
            {
                var status = new MP4Service.Contract.Client.DataTask
                {
                    Id = Guid.Empty,
                };

                if (!string.IsNullOrWhiteSpace(mp4LicenseKey))
                {
                    CheckStatus(mp4Client, Guid.Parse(mp4LicenseKey), recordingScoId, status, logger);
                }

                if (!string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey) && (status.Id == Guid.Empty))
                {
                    CheckStatus(mp4Client, Guid.Parse(mp4WithSubtitlesLicenseKey), recordingScoId, status, logger);
                }
                
                if (status.Duration == -777)
                {
                    return OperationResultWithData<Mp4TaskStatusDto>.Success(new Mp4TaskStatusDto
                    {
                        status = "MP4 Service Error",
                    });
                }
                
                return OperationResultWithData<Mp4TaskStatusDto>.Success(new Mp4TaskStatusDto
                {
                    mp4_sco_id = (status.Status >= MP4Service.Contract.Client.TaskStatus.Uploaded) ? status.UploadScoId : null,
                    cc_sco_id = (status.Status >= MP4Service.Contract.Client.TaskStatus.Transcripted) ? status.TranscriptScoId : null,
                    status = status.Status.ToString(),
                });
            }
            return OperationResultWithData<Mp4TaskStatusDto>.Error("No MP4 license found");
        }

        private static void CheckStatus(TaskClient mp4Client, Guid mp4LicenseKey, long recordingScoId, MP4Service.Contract.Client.DataTask task,
            ILogger logger)
        {
            //if (mp4Client == null)
            //    throw new ArgumentNullException("mp4Client");
            //if (logger == null)
            //    throw new ArgumentNullException("logger");

            try
            {
                var status = mp4Client.GetStatus(new MP4Service.Contract.Client.TaskParam
                {
                    LicenseId = mp4LicenseKey.ToString(),
                    ScoId = recordingScoId,
                }).Result;

                if (status != null)
                {
                    task.Id = status.Id;
                    task.ScoId = status.ScoId;
                    task.UploadScoId = status.UploadScoId;
                    task.LicenseId = status.LicenseId;
                    task.Modified = status.Modified;
                    task.Duration = status.Duration;
                    task.Status = status.Status;
                    task.TranscriptScoId = status.TranscriptScoId;
                }
            }
            catch (AggregateException ex)
            {
                ProcessAggregateException(ex, string.Format("CheckStatus-mp4. License Key: {0}, ScoId:{1}", mp4LicenseKey, recordingScoId), logger);
                // TRICK: for error handling
                task.Duration = -777;
            }
            catch (ApiException ex)
            {
                ProcessApiException(ex, string.Format("CheckStatus-mp4. License Key: {0}, ScoId:{1}", mp4LicenseKey, recordingScoId), logger);
                // TRICK: for error handling
                task.Duration = -777;
            }
            catch (HttpRequestException ex)
            {
                logger.ErrorFormat(ex, "CheckStatus-mp4. License Key: {0}, ScoId:{1}", mp4LicenseKey, recordingScoId);
                // TRICK: for error handling
                task.Duration = -777;
            }
        }


        public static OperationResult ProcessAggregateException(AggregateException ex, string baseMessage, ILogger logger)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");
            if (logger == null)
                throw new ArgumentNullException("logger");

            logger.Error("Mp4ApiUtility AggregateException", ex);
            foreach (ApiException exception in ex.InnerExceptions.Where(x => x is ApiException))
            {
                return ProcessApiException(exception, baseMessage, logger);
            }

            foreach (Exception exception in ex.InnerExceptions)
            {
                logger.Error("Mp4ApiUtility AggregateException - InnerExceptions", ex);
                return OperationResult.Error(baseMessage + exception.Message);
            }

            return OperationResult.Error(baseMessage + ex.Message);
        }

        public static OperationResult ProcessApiException(ApiException ex, string baseMessage, ILogger logger)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");
            if (logger == null)
                throw new ArgumentNullException("logger");

            logger.ErrorFormat(ex, "Mp4ApiUtility ApiException. Message:{0}. ErrorDetails:{1}. Response.Content:{2}.",
                ex.Message,
                ex.ErrorDetails.ToString(), 
                ex.Response.Content.ReadAsStringAsync().Result);

            return OperationResult.Error(baseMessage + (ex.ErrorDetails != null ? ex.ErrorDetails.ToString() : ex.Message));
        }

    }

}
