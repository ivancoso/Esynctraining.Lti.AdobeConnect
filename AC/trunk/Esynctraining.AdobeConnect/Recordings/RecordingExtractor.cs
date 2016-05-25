using System;
using System.Collections.Generic;
using System.Linq;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class RecordingExtractor : RecordingExtractorBase
    {
        public RecordingExtractor(IAdobeConnectProxy acProxy) : base(acProxy)
        {
        }


        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone)
        {
            return GetRecordings(dtoBuilder, scoId, accountUrl, timeZone, 0, int.MaxValue);
        }

        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone, int skip, int take)
        {
            var result = new List<IRecordingDto>();
            var apiRecordings = AcProxy.GetRecordingsList(scoId);
            foreach (var recording in apiRecordings.Values.Where(rec => rec.Icon != "mp4-archive").Skip(skip).Take(take))
            {
                var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                dto.IsPublic = IsPublicRecording(recording.ScoId);
                result.Add(dto);
            }

            return result;
        }

    }

}
