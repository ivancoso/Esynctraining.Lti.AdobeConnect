using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;

namespace Esynctraining.Lti.Zoom.Common.Services.MeetingLoader
{
    public class StudyGroupMeetingsLoader : MeetingsLoader
    {
        protected readonly string _email;

        public StudyGroupMeetingsLoader(
                                        ZoomDbContext dbContext, 
                                        LmsLicenseDto license, 
                                        string courseId, 
                                        ZoomApiWrapper zoomApi, 
                                        string currentUserId, 
                                        string email, 
                                        UserInfoDto user, 
                                        ZoomUserService zoomUserService) 
                                        : base(dbContext, license, courseId, zoomApi, currentUserId, user, zoomUserService)
        {
            CourseMeetingType = CourseMeetingType.StudyGroup;
            _email = email;
        }

        protected override async Task<IEnumerable<MeetingViewModel>> ProcessMergedMeetings(List<MeetingViewModel> meetings)
        {
            IList<MeetingViewModel> result = new List<MeetingViewModel>();

            var sgMeetings = meetings.Where(x => x.Type == (int)CourseMeetingType.StudyGroup);

            foreach (var meeting in sgMeetings)
            {
                var zoomListRegistrants = await _zoomApi.GetMeetingRegistrants(meeting.ConferenceId, null, nameof(ZoomMeetingRegistrantStatus.Approved));
                if (zoomListRegistrants.Registrants.Any(r => r.Email == _email) || meeting.HostId == _currentUserId)
                {
                    result.Add(meeting);
                }
            }

            return result;
        }
    }
}
