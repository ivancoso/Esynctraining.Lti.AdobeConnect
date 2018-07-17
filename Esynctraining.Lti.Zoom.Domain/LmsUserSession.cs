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
        public string CourseId { get; set; }
        public string SessionData { get; set; } //json LtiParamDTO
        //public virtual LtiParamDTO LtiSession
        //{
        //    get
        //    {
        //        return !string.IsNullOrWhiteSpace(this.SessionData)
        //            ? JsonConvert.DeserializeObject<LtiParamDTO>(this.SessionData)
        //            : null;
        //    }
        //}
    }
}