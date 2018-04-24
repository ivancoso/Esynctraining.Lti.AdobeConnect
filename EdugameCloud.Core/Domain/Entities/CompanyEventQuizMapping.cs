using System;
using System.Collections.Generic;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.Entities
{
    public class CompanyEventQuizMapping : Entity
    {
        public CompanyEventQuizMapping()
        {
            Results = new List<QuizResult>();
        }

        public virtual string AcEventScoId { get; set; }
        public virtual Quiz PreQuiz { get; set; }
        public virtual Quiz PostQuiz { get; set; }
        public virtual CompanyAcServer CompanyAcDomain { get; set; }

        public virtual Guid Guid { get; set; }
        public virtual IList<QuizResult> Results { get; protected set; }

    }
}