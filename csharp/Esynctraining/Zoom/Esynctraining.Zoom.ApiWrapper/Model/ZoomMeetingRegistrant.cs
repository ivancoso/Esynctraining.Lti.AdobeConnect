using System;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomMeetingRegistrant
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public string JoinUrl { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
    }
}