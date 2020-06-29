using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class School
    {
        public int SchoolId { get; set; }
        public int StateId { get; set; }
        public string SchoolNumber { get; set; }
        public string OnsiteOperator { get; set; }
        public string FirstDirector { get; set; }
        public string MainPhone { get; set; }
        public string Fax { get; set; }
        public string SpeedDialNumber { get; set; }
        public string SchoolEmail { get; set; }
        public string CorporateName { get; set; }
        public string Fbcrepresentative { get; set; }
        public string Essrepresentative { get; set; }
        public string StandardsRepresentative { get; set; }
        public string AdvRepresentative { get; set; }
        public string MktgRepresentative { get; set; }
        public string AccountName { get; set; }

        public virtual State State { get; set; }
    }
}
