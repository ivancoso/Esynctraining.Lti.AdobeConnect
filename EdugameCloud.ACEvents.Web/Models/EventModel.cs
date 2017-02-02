using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Models
{
    public class EventModel
    {
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string School { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }
    }
}