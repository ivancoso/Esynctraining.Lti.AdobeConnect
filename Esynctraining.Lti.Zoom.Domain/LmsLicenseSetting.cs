namespace Esynctraining.Lti.Zoom.Domain
{
    public class LmsLicenseSetting : BaseEntity
    {
        public LmsLicense License { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}