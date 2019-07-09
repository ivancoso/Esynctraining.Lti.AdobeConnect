using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameClaud.Lti.SearchApi.Host.DTOs;
using EdugameClaud.Lti.SearchApi.Host.Models;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameClaud.Lti.SearchApi.Host.Controllers
{
    [Route("meetings-search")]
    [ApiController]
    public class MeetingSearchController : ControllerBase
    {
        private IJsonDeserializer _jsonDeserializer = null;
        private readonly EduGameCloudDbContext _dbContext = null;
        public MeetingSearchController(EduGameCloudDbContext dbContext, IJsonDeserializer jsonDeserializer)
        {
            _dbContext = dbContext;
            _jsonDeserializer = jsonDeserializer;
        }

        [Route("")]
        [HttpGet]
        public async Task<OperationResultWithData<IEnumerable<MeetingDto>>> FindMeetingByText(string consumerKey, string text)
        {
            CompanyLms license = _dbContext.CompanyLms.First(c => c.ConsumerKey == consumerKey);
            int licenseId = license.CompanyLmsId;

            var couseMeetings = _dbContext.LmsCourseMeeting.Where(cm => cm.CompanyLmsId == licenseId).ToList();
            var lmsCourseMeetingDtos = couseMeetings.Select(cm => new LmsCourseMeetingDto {
                LmsCourseMeetingId = cm.LmsCourseMeetingId,
                CompanyLmsId = cm.CompanyLmsId,
                CourseId = cm.CourseId,
                ScoId = cm.ScoId,
                MeetingNameInfo = _jsonDeserializer.JsonDeserialize<MeetingNameInfoDto>(cm.MeetingNameJson)
            });

            var foundCoursesIds = lmsCourseMeetingDtos.Where(r => r.MeetingNameInfo.courseNum.Contains(text, StringComparison.OrdinalIgnoreCase)).Select(f => f.LmsCourseMeetingId);
            var foundMeetingsIds = lmsCourseMeetingDtos.Where(r => r.MeetingNameInfo.meetingName.Contains(text, StringComparison.OrdinalIgnoreCase)).Select(f => f.LmsCourseMeetingId);

            var resultIds = new List<int>();
            resultIds.AddRange(foundCoursesIds);
            resultIds.AddRange(foundMeetingsIds);
            resultIds = resultIds.Distinct().ToList();

            IEnumerable<MeetingDto> result = lmsCourseMeetingDtos.Where(r => resultIds.Contains(r.LmsCourseMeetingId)).Select(
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