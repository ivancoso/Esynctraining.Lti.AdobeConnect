using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EdugameCloud.ACEvents.Web.Models
{
    public class EventModel
    {
        public int EventQuizMappingId { get; set; }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage= "Should contain only letters or numbers")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Should contain only letters or numbers")]
        public string LastName { get; set; }
        [Required]
        [MinLength(4)]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Should contain only letters or numbers")]
        public string Password { get; set; }
        [Required]
        [MinLength(4)]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Should contain only letters or numbers")]
        public string VerifyPassword { get; set; }
        [Required]
        public string State { get; set; }
        public string Country { get; set; }
        [Required]
        public string School { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }
    }
}