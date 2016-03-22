using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class RecordingsExtractor : RecordingsExtractorBase
    {
        public RecordingsExtractor(Esynctraining.AdobeConnect.IAdobeConnectProxy acProxy) : base(acProxy)
        {
        }

        public override List<RecordingDTO> GetRecordings(string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            var result = new List<RecordingDTO>();
            var apiRecordings = AcProxy.GetRecordingsList(scoId);
            foreach (var recording in apiRecordings.Values)
            {
                var isPublic = IsPublicRecording(recording.ScoId);

                result.Add(new RecordingDTO(recording, accountUrl, timeZone)
                {
                    is_public = isPublic
                });
            }

            return result;
        }
    }
}
