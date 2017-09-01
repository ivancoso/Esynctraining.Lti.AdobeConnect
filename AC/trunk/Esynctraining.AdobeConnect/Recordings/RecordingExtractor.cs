using System;
using System.Collections.Generic;
using Esynctraining.AC.Provider;

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
            var apiRecordings = AcProxy.GetRecordingsList(scoId, 0, int.MaxValue, "date-begin", SortOrder.Descending, excludeMp4: true);
            
            foreach (var recording in apiRecordings.Values)
            {
                var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                result.Add(dto);
            }

            return result;
        }

    }

}
