using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class SeminarRecordingExtractor : RecordingExtractorBase
    {
        private readonly ISeminarService _seminarService;
        private readonly ILogger _logger;


        /// <summary>
        /// Default value: 1.
        /// </summary>
        public static int MaxDegreeOfParallelism { get; set; } = 1;


        public SeminarRecordingExtractor(IAdobeConnectProxy acProxy, ISeminarService seminarService, ILogger logger) : base(acProxy)
        {
            _seminarService = seminarService ?? throw new ArgumentNullException(nameof(seminarService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            var result = new List<IRecordingDto>();
            var seminarRecordings = AcProxy.GetRecordingsList(scoId);
            var seminarSessions = _seminarService.GetSeminarSessions(scoId, AcProxy);

            if (MaxDegreeOfParallelism > 1)
            {
                object localLockObject = new object();

                Parallel.ForEach(
                      seminarSessions,
                      new ParallelOptions { MaxDegreeOfParallelism = 20, },
                      () => { return new List<IRecordingDto>(); },
                      (seminarSession, state, localList) =>
                      {
                          ProcessSession(AcProxy,
                              scoId,
                              seminarSession,
                              dtoBuilder,
                              accountUrl,
                              timeZone,
                              localList);

                          return localList;
                      },
                      (finalResult) => { lock (localLockObject) result.AddRange(finalResult); }
                );
            }
            else
            {
                foreach (var seminarSession in seminarSessions)
                {
                    ProcessSession(AcProxy,
                        scoId,
                        seminarSession,
                        dtoBuilder,
                        accountUrl,
                        timeZone,
                        result);
                }
            }

            var recordingsWithoutSession = seminarRecordings.Values
                .Where(x => x.Icon != "mp4-archive")
                .Where(x => result.All(r => !r.Id.Equals(x.ScoId)))
                .Select(x =>
                {
                    var dto = dtoBuilder.Build(x, accountUrl, timeZone);
                    return dto;
                })
                .ToList();

            result.AddRange(recordingsWithoutSession);
            
            return result;
        }


        private void ProcessSession(IAdobeConnectProxy AcProxy, 
            string seminarScoId,
            AC.Provider.Entities.ScoContent seminarSession,
            IRecordingDtoBuilder dtoBuilder,
            string accountUrl, 
            TimeZoneInfo timeZone,
            List<IRecordingDto> resultList)
        {
            RecordingCollectionResult sessionRecordings = null;
            // VSSO-205
            // https://verizon.cosocloud.com/api/xml?action=list-recordings&folder-id=7349002&seminar-session-id=8766014 returns internal-error, catching error
            try
            {
                sessionRecordings = AcProxy.GetSeminarSessionRecordingsList(seminarScoId, seminarSession.ScoId);
            }
            catch(AdobeConnectException e) 
            {
                _logger.Error($"[Error-GetSeminarSessionRecordingsList] Server={AcProxy.AdobeConnectRoot}, seminarId={seminarScoId}, sessionId={seminarSession.ScoId}, AC response: {e.Status?.InnerXml}");
                return;
            }

            foreach (var recording in sessionRecordings.Values.Where(x => x.Icon != "mp4-archive"))
            {
                var dto = dtoBuilder.Build(recording, accountUrl, timeZone);

                ISeminarSessionRecordingDto seminarRecording = dto as ISeminarSessionRecordingDto;
                if (seminarRecording == null)
                    throw new InvalidOperationException("ISeminarSessionRecordingDto expected");
                seminarRecording.SeminarSessionId = seminarSession.ScoId;
                seminarRecording.SeminarSessionName = seminarSession.Name;

                resultList.Add(dto);
            }
        }
    }

}
