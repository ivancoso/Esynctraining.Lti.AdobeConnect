using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class CompanyEventQuizMapping
    {
        public CompanyEventQuizMapping()
        {
            QuizResult = new HashSet<QuizResult>();
        }

        public int CompanyEventQuizMappingId { get; set; }
        public int? PreQuizId { get; set; }
        public int? PostQuizId { get; set; }
        public int CompanyAcDomainId { get; set; }
        public string AcEventScoId { get; set; }
        public Guid Guid { get; set; }

        public virtual CompanyAcDomains CompanyAcDomain { get; set; }
        public virtual Quiz PostQuiz { get; set; }
        public virtual Quiz PreQuiz { get; set; }
        public virtual ICollection<QuizResult> QuizResult { get; set; }
    }
}
