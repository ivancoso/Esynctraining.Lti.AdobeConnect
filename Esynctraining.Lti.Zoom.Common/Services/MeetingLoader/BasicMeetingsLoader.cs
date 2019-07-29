﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;

namespace Esynctraining.Lti.Zoom.Common.Services.MeetingLoader
{
    public class BasicMeetingsLoader : MeetingsLoader
    {
        public BasicMeetingsLoader(ZoomDbContext dbContext, Guid licenseKey, string courseId, ZoomApiWrapper zoomApi, string currentUserId, UserInfoDto user) 
            : base(dbContext, licenseKey, courseId, zoomApi, currentUserId, user)
        {
            CourseMeetingType = CourseMeetingType.Basic;
        }

        protected override async Task<IEnumerable<MeetingViewModel>> ProcessMergedMeetings(List<MeetingViewModel> result)
        {
            return result.Where(meeting => meeting.Type == (int)CourseMeetingType.Basic).ToList();
        }

    }
}
