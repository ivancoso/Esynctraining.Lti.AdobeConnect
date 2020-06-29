using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsUser")]
    public class LmsUser : BaseEntity
    {
        public int LicenseId { get; set; }
        public string Email { get; set; }
        public string LmsUserId { get; set; }
        public string LmsUsername { get; set; }
        public string LmsToken { get; set; }
        public string ProviderUserId { get; set; } //Zoom Id
    }
}