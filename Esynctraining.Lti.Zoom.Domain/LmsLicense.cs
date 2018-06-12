using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Domain
{
    public class LmsLicense : BaseEntity
    {
        public string ConsumerKey { get; set; }
        public string SharedSecret { get; set; }
        public string LmsDomain { get; set; }
        public virtual IEnumerable<LmsLicenseSetting> Settings { get; set; }
    }
}