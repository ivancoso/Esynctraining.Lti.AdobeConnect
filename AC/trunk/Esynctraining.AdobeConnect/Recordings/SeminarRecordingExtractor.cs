using System;
using System.Collections.Generic;
using System.Linq;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class SeminarRecordingExtractor : RecordingExtractorBase
    {
        private readonly ISeminarService seminarService;
        

        public SeminarRecordingExtractor(IAdobeConnectProxy acProxy, ISeminarService seminarService) : base(acProxy)
        {
            this.seminarService = seminarService;
        }


        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            var result = new List<IRecordingDto>();
            var seminarRecordings = AcProxy.GetRecordingsList(scoId);
            var seminarSessions = seminarService.GetSeminarSessions(scoId, AcProxy);
            foreach (var seminarSession in seminarSessions)
            {
                var sessionRecordings = AcProxy.GetSeminarSessionRecordingsList(scoId, seminarSession.ScoId);
                foreach (var recording in sessionRecordings.Values)
                {
                    var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                    dto.IsPublic = IsPublicRecording(recording.ScoId);

                    ISeminarSessionRecordingDto seminarRecording = dto as ISeminarSessionRecordingDto;
                    if (seminarRecording == null)
                        throw new InvalidOperationException("ISeminarSessionRecordingDto expected");
                    seminarRecording.SeminarSessionId = seminarSession.ScoId;
                    seminarRecording.SeminarSessionName = seminarSession.Name;
                }
            }

            var recordingsWithoutSession = seminarRecordings.Values
                .Where(x => result.All(r => !r.Id.Equals(x.ScoId)))
                .Select(x => {
                    var dto = dtoBuilder.Build(x, accountUrl, timeZone);
                    dto.IsPublic = IsPublicRecording(x.ScoId);
                    return dto;
                })
                .ToList();

            result.AddRange(recordingsWithoutSession);

            return result;
        }

    }

}
