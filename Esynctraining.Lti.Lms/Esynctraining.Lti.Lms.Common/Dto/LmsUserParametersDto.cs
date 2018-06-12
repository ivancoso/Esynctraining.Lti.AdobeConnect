using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Common.Dto
{
    public class LmsUserParametersDto
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Token { get; set; }
        public Dictionary<string, object> LicenseSettings { get; set; }
    }
}