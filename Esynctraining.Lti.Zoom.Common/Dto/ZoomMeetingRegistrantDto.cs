using System;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;

namespace Esynctraining.Lti.Zoom.Common.Dto
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