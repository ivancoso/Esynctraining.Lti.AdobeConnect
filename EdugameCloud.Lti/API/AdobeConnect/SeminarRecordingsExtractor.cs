using System.Linq;
using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using System;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class SeminarRecordingsExtractor : RecordingsExtractorBase
    {
        private readonly ISeminarService seminarService;
        public SeminarRecordingsExtractor(Esynctraining.AdobeConnect.IAdobeConnectProxy acProxy, ISeminarService seminarService) : base(acProxy)
        {
            this.seminarService = seminarService;
        }

        public override List<RecordingDTO> GetRecordings(string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            var result = new List<RecordingDTO>();
            var seminarRecordings = AcProxy.GetRecordingsList(scoId);
            var seminarSessions = seminarService.GetSeminarSessions(scoId, AcProxy);
            foreach (var seminarSession in seminarSessions)
            {
                var sessionRecordings = AcProxy.GetSeminarSessionRecordingsList(scoId, seminarSession.ScoId);
                foreach (var recording in sessionRecordings.Values)
                {
                    var isPublic = IsPublicRecording(recording.ScoId);

                    result.Add(new SeminarSessionRecordingDto(recording, accountUrl, timeZone)
                    {
                        is_public = isPublic,
                        seminarSessionId = seminarSession.ScoId,
                        seminarSessionName = seminarSession.Name
                    });
                }
            }

            var recordingsWithoutSession = seminarRecordings.Values
                .Where(x => result.All(r => !r.id.Equals(x.ScoId)))
                .Select(x => new SeminarSessionRecordingDto(x, accountUrl, timeZone)
                {
                    is_public = IsPublicRecording(x.ScoId)
                })
                .ToList();

            result.AddRange(recordingsWithoutSession);

            return result;
        }

    }

}
