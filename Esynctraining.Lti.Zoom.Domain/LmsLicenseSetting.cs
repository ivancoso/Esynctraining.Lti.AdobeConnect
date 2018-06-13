using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsLicenseSetting")]
    public class LmsLicenseSetting : BaseEntity
    {
        public virtual LmsLicense License { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}