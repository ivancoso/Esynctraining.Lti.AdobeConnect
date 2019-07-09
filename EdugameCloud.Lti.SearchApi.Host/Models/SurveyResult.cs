using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class SurveyResult
    {
        public SurveyResult()
        {
            SurveyQuestionResult = new HashSet<SurveyQuestionResult>();
        }

        public int SurveyResultId { get; set; }
        public int SurveyId { get; set; }
        public int AcSessionId { get; set; }
        public string ParticipantName { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsArchive { get; set; }
        public string Email { get; set; }
        public int? LmsUserParametersId { get; set; }
        public string AcEmail { get; set; }

        public virtual Acsession AcSession { get; set; }
        public virtual LmsUserParameters LmsUserParameters { get; set; }
        public virtual ICollection<SurveyQuestionResult> SurveyQuestionResult { get; set; }
    }
}
