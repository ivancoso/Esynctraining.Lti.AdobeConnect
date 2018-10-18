using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsUserSession")]
    public class LmsUserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid LicenseKey { get; set; }
        public string Email { get; set; }
        public string LmsUserId { get; set; }
        public string Token { get; set; }
        [MaxLength(200)]
        public string RefreshToken { get; set; }
        public string CourseId { get; set; }
        public string SessionData { get; set; }
    }
}