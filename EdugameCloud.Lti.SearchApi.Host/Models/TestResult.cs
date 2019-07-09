using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class TestResult
    {
        public TestResult()
        {
            TestQuestionResult = new HashSet<TestQuestionResult>();
        }

        public int TestResultId { get; set; }
        public int TestId { get; set; }
        public int AcSessionId { get; set; }
        public string ParticipantName { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsArchive { get; set; }
        public string Email { get; set; }
        public string AcEmail { get; set; }
        public bool? IsCompleted { get; set; }

        public virtual Acsession AcSession { get; set; }
        public virtual Test Test { get; set; }
        public virtual ICollection<TestQuestionResult> TestQuestionResult { get; set; }
    }
}
