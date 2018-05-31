using System;
using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomAddRegistrantRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ZoomAddRegistrantResponse
    {
        public string RegistrantId { get; set; }
        public string Id { get; set; }
        public string Topic { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string JoinUrl { get; set; }
    }

    public class ZoomRegistrantForStatusRequest
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }

    public class ZoomUpdateRegistrantStatusRequest
    {
        public string Action { get; set; }
        public List<ZoomRegistrantForStatusRequest> Registrants { get; set; }
    }
}