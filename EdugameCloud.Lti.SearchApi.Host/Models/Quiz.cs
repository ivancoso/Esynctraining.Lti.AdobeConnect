using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            CompanyEventQuizMappingPostQuiz = new HashSet<CompanyEventQuizMapping>();
            CompanyEventQuizMappingPreQuiz = new HashSet<CompanyEventQuizMapping>();
            QuizResult = new HashSet<QuizResult>();
        }

        public int QuizId { get; set; }
        public int? SubModuleItemId { get; set; }
        public int? QuizFormatId { get; set; }
        public int? ScoreTypeId { get; set; }
        public string QuizName { get; set; }
        public string Description { get; set; }
        public int? LmsQuizId { get; set; }
        public int? LmsProviderId { get; set; }
        public bool? IsPostQuiz { get; set; }
        public int PassingScore { get; set; }
        public Guid Guid { get; set; }

        public virtual LmsProvider LmsProvider { get; set; }
        public virtual QuizFormat QuizFormat { get; set; }
        public virtual ScoreType ScoreType { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual ICollection<CompanyEventQuizMapping> CompanyEventQuizMappingPostQuiz { get; set; }
        public virtual ICollection<CompanyEventQuizMapping> CompanyEventQuizMappingPreQuiz { get; set; }
        public virtual ICollection<QuizResult> QuizResult { get; set; }
    }
}
