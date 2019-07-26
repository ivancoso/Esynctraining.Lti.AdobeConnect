using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsUserParameters
    {
        public LmsUserParameters()
        {
            SurveyResult = new HashSet<SurveyResult>();
        }

        public int LmsUserParametersId { get; set; }
        public string Wstoken { get; set; }
        public string Course { get; set; }
        public string AcId { get; set; }
        public int? LmsUserId { get; set; }
        public int? CompanyLmsId { get; set; }
        public string CourseName { get; set; }
        public string UserEmail { get; set; }
        public DateTime? LastLoggedIn { get; set; }

        public virtual CompanyLms CompanyLms { get; set; }
        public virtual LmsUser LmsUser { get; set; }
        public virtual ICollection<SurveyResult> SurveyResult { get; set; }
    }
}
