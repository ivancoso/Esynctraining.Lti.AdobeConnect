using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuizResult
    {
        public QuizResult()
        {
            QuizQuestionResult = new HashSet<QuizQuestionResult>();
        }

        public int QuizResultId { get; set; }
        public int QuizId { get; set; }
        public int AcSessionId { get; set; }
        public string ParticipantName { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsArchive { get; set; }
        public string Email { get; set; }
        public int? LmsId { get; set; }
        public string AcEmail { get; set; }
        public bool? IsCompleted { get; set; }
        public int? LmsUserParametersId { get; set; }
        public int? EventQuizMappingId { get; set; }
        public int? AppMaximizedTime { get; set; }
        public int? AppInFocusTime { get; set; }
        public Guid Guid { get; set; }

        public virtual Acsession AcSession { get; set; }
        public virtual CompanyEventQuizMapping EventQuizMapping { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizQuestionResult> QuizQuestionResult { get; set; }
    }
}
