using System;
using System.Collections.Generic;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class RecordingExtractor : RecordingExtractorBase
    {
        public RecordingExtractor(IAdobeConnectProxy acProxy) : base(acProxy)
        {
        }

        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            var result = new List<IRecordingDto>();
            var apiRecordings = AcProxy.GetRecordingsList(scoId);
            foreach (var recording in apiRecordings.Values)
            {
                var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                dto.IsPublic = IsPublicRecording(recording.ScoId);
                result.Add(dto);
            }

            return result;
        }

    }

}
