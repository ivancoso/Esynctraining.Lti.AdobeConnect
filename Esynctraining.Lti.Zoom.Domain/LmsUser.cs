namespace Esynctraining.Lti.Zoom.Domain
{
    public class LmsUser : BaseEntity
    {
        public int LicenseId { get; set; }
        public string LmsId { get; set; }
        public string Email { get; set; }
        public string LmsToken { get; set; }
        public string ProviderUserId { get; set; } //Zoom Id
    }
}