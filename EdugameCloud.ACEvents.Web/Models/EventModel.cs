using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EdugameCloud.ACEvents.Web.Models
{
    public class EventModel
    {
        public string EventScoId { get; set; }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [MinLength(4)]
        public string Password { get; set; }
        [Required]
        [MinLength(4)]
        public string VerifyPassword { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [Required]
        public string School { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }
    }
}