using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Schoology
{
    internal class EnrollmentsResult
    {
        public List<Enrollment> enrollment { get; set; }
        public string total { get; set; }
        public Links links { get; set; }
    }
}
