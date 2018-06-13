using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsLicense")]
    public class LmsLicense : BaseEntity
    {
        public string ConsumerKey { get; set; }
        public string SharedSecret { get; set; }
        public int LmsProviderId { get; set; }
        public string Domain { get; set; }
        public virtual IEnumerable<LmsLicenseSetting> Settings { get; set; }
    }
}