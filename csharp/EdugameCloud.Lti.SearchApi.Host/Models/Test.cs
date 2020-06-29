using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Test
    {
        public Test()
        {
            TestResult = new HashSet<TestResult>();
        }

        public int TestId { get; set; }
        public int? SubModuleItemId { get; set; }
        public int? ScoreTypeId { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public decimal? PassingScore { get; set; }
        public int? TimeLimit { get; set; }
        public string InstructionTitle { get; set; }
        public string InstructionDescription { get; set; }
        public string ScoreFormat { get; set; }

        public virtual ScoreType ScoreType { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual ICollection<TestResult> TestResult { get; set; }
    }
}
