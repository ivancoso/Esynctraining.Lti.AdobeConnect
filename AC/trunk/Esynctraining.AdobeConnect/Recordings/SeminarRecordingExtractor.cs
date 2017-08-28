using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esynctraining.AdobeConnect.Recordings
{
    public class SeminarRecordingExtractor : RecordingExtractorBase
    {
        private readonly ISeminarService seminarService;
        

        public SeminarRecordingExtractor(IAdobeConnectProxy acProxy, ISeminarService seminarService) : base(acProxy)
        {
            this.seminarService = seminarService;
        }


        public override PagedResult<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone,
            string sortBy, string sortOrder, string search, long? dateFrom, long? dateTo, int skip, int take)
        {
            var result = new List<IRecordingDto>();
            var seminarRecordings = AcProxy.GetRecordingsList(scoId);
            var seminarSessions = seminarService.GetSeminarSessions(scoId, AcProxy);
            foreach (var seminarSession in seminarSessions)
            {
                var sessionRecordings = AcProxy.GetSeminarSessionRecordingsList(scoId, seminarSession.ScoId);
                foreach (var recording in sessionRecordings.Values.Where(x => x.Icon != "mp4-archive"))
                {
                    var dto = dtoBuilder.Build(recording, accountUrl, timeZone);
                    //dto.IsPublic = IsPublicRecording(recording.ScoId);

                    ISeminarSessionRecordingDto seminarRecording = dto as ISeminarSessionRecordingDto;
                    if (seminarRecording == null)
                        throw new InvalidOperationException("ISeminarSessionRecordingDto expected");
                    seminarRecording.SeminarSessionId = seminarSession.ScoId;
                    seminarRecording.SeminarSessionName = seminarSession.Name;

                    result.Add(dto);
                }
            }

            var recordingsWithoutSession = seminarRecordings.Values
                .Where(x => x.Icon != "mp4-archive")
                .Where(x => result.All(r => !r.Id.Equals(x.ScoId)))
                .Select(x =>
                {
                    var dto = dtoBuilder.Build(x, accountUrl, timeZone);
                    //dto.IsPublic = IsPublicRecording(x.ScoId);
                    return dto;
                })
                .ToList();

            result.AddRange(recordingsWithoutSession);

            IEnumerable<IRecordingDto> resultDto = result;
            resultDto = ApplyFilter(search, dateFrom, dateTo, resultDto);
            resultDto = ApplySort(sortBy, sortOrder, resultDto);

            var total = resultDto.Count();

            resultDto = resultDto
                .Skip(skip)
                .Take(take)
                .ToList();

            Parallel.ForEach(resultDto, (recording) =>
            {
                recording.IsPublic = IsPublicRecording(recording.Id);
            });

            var pagedResult = new PagedResult<IRecordingDto> { Data = resultDto, Total = total, Skip = skip, Take = take };

            return pagedResult;
        }

        private static IEnumerable<IRecordingDto> ApplySort(string sortBy, string sortOrder, IEnumerable<IRecordingDto> resultDto)
        {
            // sorting
            bool isDescendingSortOrder = (sortOrder ?? "").StartsWith("desc", StringComparison.OrdinalIgnoreCase);

            switch (sortBy)
            {
                case "name":
                    resultDto = isDescendingSortOrder
                        ? resultDto.OrderByDescending(x => x.Name)
                        : resultDto.OrderBy(x => x.Name);

                    break;
                case "duration":

                    resultDto = isDescendingSortOrder
                        ? resultDto.OrderByDescending(x => x.Duration)
                        : resultDto.OrderBy(x => x.Duration);

                    break;
                case "date-created":

                    resultDto = isDescendingSortOrder
                       ? resultDto.OrderByDescending(x => x.BeginAt)
                       : resultDto.OrderBy(x => x.BeginAt);

                    break;
                default:

                    resultDto = resultDto.OrderByDescending(x => x.BeginAt);

                    break;
            }

            return resultDto;
        }

        private static IEnumerable<IRecordingDto> ApplyFilter(string search, long? dateFrom, long? dateTo, IEnumerable<IRecordingDto> resultDto)
        {
            // filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                resultDto = resultDto.Where(x => x.Name.Contains(search));
            }

            if (dateFrom.HasValue)
            {
                resultDto = resultDto.Where(x => x.BeginAt >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                resultDto = resultDto.Where(x => x.BeginAt <= dateTo.Value);
            }

            return resultDto;
        }
    }

}
