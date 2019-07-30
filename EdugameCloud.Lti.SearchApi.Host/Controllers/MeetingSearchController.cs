using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameClaud.Lti.SearchApi.Host.DTOs;
using EdugameClaud.Lti.SearchApi.Host.Models;
using EdugameCloud.Lti.SearchApi.Host.Models;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameClaud.Lti.SearchApi.Host.Controllers
{
    [Route("meetings-search")]
    [ApiController]
    public class MeetingSearchController : ControllerBase
    {
        private IJsonDeserializer _jsonDeserializer = null;
        private readonly EduGameCloudContext _dbContext = null;
        private ILogger _logger { get; set; }

        public MeetingSearchController(EduGameCloudContext dbContext, IJsonDeserializer jsonDeserializer, ILogger logger)
        {
            _dbContext = dbContext;
            _jsonDeserializer = jsonDeserializer;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<OperationResultWithData<IEnumerable<MeetingDto>>> FindMeetingByText(string consumerKey, string text)
        {
            _logger.InfoFormat("Consumer key: {0}, text: {1}", consumerKey, text);

            CompanyLms license = _dbContext.CompanyLms.First(c => c.ConsumerKey == consumerKey);
            int licenseId = license.CompanyLmsId;

            var couseMeetings = _dbContext.LmsCourseMeeting.Where(cm => cm.CompanyLmsId == licenseId).ToList();
            var lmsCourseMeetingDtos = couseMeetings.Select(cm => new LmsCourseMeetingDto {
                LmsCourseMeetingId = cm.LmsCourseMeetingId,
                CompanyLmsId = cm.CompanyLmsId,
                CourseId = cm.CourseId,
                ScoId = cm.ScoId,
                OfficeHoursId = cm.OfficeHoursId,
                MeetingNameInfo = _jsonDeserializer.JsonDeserialize<MeetingNameInfoDto>(cm.MeetingNameJson)
            });

            var foundCoursesIds = lmsCourseMeetingDtos.Where(r => r.MeetingNameInfo.courseNum.Contains(text, StringComparison.OrdinalIgnoreCase)).Select(f => f.LmsCourseMeetingId);
            var foundMeetingsIds = lmsCourseMeetingDtos.Where(r => r.MeetingNameInfo.meetingName.Contains(text, StringComparison.OrdinalIgnoreCase)).Select(f => f.LmsCourseMeetingId);

            var resultIds = new List<int>();
            resultIds.AddRange(foundCoursesIds);
            resultIds.AddRange(foundMeetingsIds);
            resultIds = resultIds.Distinct().ToList();

            IEnumerable<LmsCourseMeetingDto> filtredMeetings = lmsCourseMeetingDtos.Where(r => resultIds.Contains(r.LmsCourseMeetingId)).ToList();

            IEnumerable<int> OfficeHoursMeetingIds = filtredMeetings
                                                        .Where(m => m.ScoId == null && m.OfficeHoursId.HasValue)
                                                        .Select(s => s.OfficeHoursId.Value)
                                                        .Distinct();

            if (OfficeHoursMeetingIds.Any())
            {
                var officeHours = _dbContext.OfficeHours.Where(oh => OfficeHoursMeetingIds.Contains(oh.OfficeHoursId));
                foreach (var officeHour in officeHours)
                {
                    var meetings = filtredMeetings.Where(m => m.OfficeHoursId == officeHour.OfficeHoursId);

                    foreach(var meeting in meetings)
                    {
                        meeting.ScoId = officeHour.ScoId;
                    }
                }
            }

            IEnumerable<MeetingDto> result = filtredMeetings.Select(
                f => new MeetingDto
                {
                    MeetingScoId = f.ScoId,
                    Name = f.MeetingNameInfo.meetingName
                }
                );

            return result.ToSuccessResult();
        }
    }
}