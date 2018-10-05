using System;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;

namespace Esynctraining.Lti.Zoom.Api.Services.MeetingLoader
{
    public class BasicMeetingsLoader : MeetingsLoader
    {
        public BasicMeetingsLoader(ZoomDbContext dbContext, Guid licenseKey, string courseId, ZoomApiWrapper zoomApi, string currentUserId) 
            : base(dbContext, licenseKey, courseId, zoomApi, currentUserId)
        {
            CourseMeetingType = CourseMeetingType.Basic;
        }

        protected override async Task<IEnumerable<MeetingViewModel>> ProcessMergedMeetings(List<MeetingViewModel> result)
        {
            return result.Where(meeting => meeting.Type == (int)CourseMeetingType.Basic).ToList();
        }

    }
}
