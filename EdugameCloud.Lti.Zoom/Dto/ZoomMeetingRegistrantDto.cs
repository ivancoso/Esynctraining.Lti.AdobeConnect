using System;
using Edugamecloud.Lti.Zoom.Dto.Enums;

namespace Edugamecloud.Lti.Zoom.Dto
{
    public class ZoomMeetingRegistrantDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ZoomMeetingRegistrantStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string JoinUrl { get; set; }
    }
}