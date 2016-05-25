using System;
using System.Collections.Generic;
using System.Linq;
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
            return GetRecordings(dtoBuilder, scoId, accountUrl, timeZone, 0, int.MaxValue);
        }

        public override IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone, int skip, int take)
        {
            return GetRecordings(dtoBuilder, scoId, accountUrl, timeZone, skip, take, "date-begin", SortOrder.Descending);
        }

        private IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone, int skip, int take, string propertySortBy, SortOrder order)
        {
            var result = new List<IRecordingDto>();
            var apiRecordings = AcProxy.GetRecordingsList(scoId, skip, take, propertySortBy, order, excludeMp4: true);
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
