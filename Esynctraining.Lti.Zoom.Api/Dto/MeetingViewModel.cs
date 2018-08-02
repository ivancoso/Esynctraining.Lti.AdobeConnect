using System;
using System.Collections.Generic;
using Esynctraining.Lti.Zoom.Api.Dto.OfficeHours;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class MeetingViewModel
    {
        public int Id { get; set; }
        public string ConferenceId { get; set; }
        //public string HostId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public long StartTime { get; set; }
        public int Duration { get; set; } //minutes
        public string Timezone { get; set; }

        public bool CanEdit { get; set; }
        public bool CanJoin { get; set; }
        public bool HasSessions { get; set; }

        public int Type { get; set; } //1 - meeting, 2 - office hours
        public string CourseId { get; set; }
        public IEnumerable<OfficeHoursTeacherAvailabilityDto> Availabilities { get; set; }
    }
}