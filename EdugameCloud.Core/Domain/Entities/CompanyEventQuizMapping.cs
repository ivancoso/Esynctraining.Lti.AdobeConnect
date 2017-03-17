using System;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.Entities
{
    public class CompanyEventQuizMapping : Entity
    {
        public virtual string AcEventScoId { get; set; }
        public virtual Quiz PreQuiz { get; set; }
        public virtual Quiz PostQuiz { get; set; }
        public virtual CompanyAcServer CompanyAcDomain { get; set; }

        public virtual Guid Guid { get; set; }

    }
}