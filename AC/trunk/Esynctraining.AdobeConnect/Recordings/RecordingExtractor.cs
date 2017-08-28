using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.AC.Provider;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class RecordingExtractor : RecordingExtractorBase
    {
        public RecordingExtractor(IAdobeConnectProxy acProxy) : base(acProxy)
        {
        }

        public override PagedResult<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone,
            string sortBy, string sortOrder, string search, long? dateFrom, long? dateTo, int skip, int take)
        {
            bool isAscendingSortOrder = (sortOrder ?? "").StartsWith("asc", StringComparison.OrdinalIgnoreCase);
            return GetRecordings(dtoBuilder, scoId, accountUrl, timeZone, skip, take, sortBy ?? "date-begin", isAscendingSortOrder ? SortOrder.Ascending : SortOrder.Descending);
        }

        private PagedResult<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone, int skip, int take, string propertySortBy, SortOrder order)
        {
            var result = new List<IRecordingDto>();
            var apiRecordings = AcProxy.GetRecordingsList(scoId, 0, int.MaxValue, propertySortBy, order, excludeMp4: true);

            var total = apiRecordings.Values.Count();

            foreach (var recording in apiRecordings.Values.Skip(skip).Take(take).ToArray())
            {
                var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                //dto.IsPublic = IsPublicRecording(recording.ScoId);
                result.Add(dto);
            }

            Parallel.ForEach(result, (recording) =>
            {
                recording.IsPublic = IsPublicRecording(recording.Id);
            });

            var pagedResult = new PagedResult<IRecordingDto> { Data = result, Total = total, Skip = skip, Take = take };

            return pagedResult;
        }

    }

}
