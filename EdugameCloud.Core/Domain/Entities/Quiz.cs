using System;

namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;

    public class Quiz : Entity
    {
        #region Public Properties

        public virtual string Description { get; set; }

        public virtual QuizFormat QuizFormat { get; set; }

        public virtual string QuizName { get; set; }

        public virtual IList<QuizResult> Results { get; protected set; }

        public virtual ScoreType ScoreType { get; set; }

        public virtual SubModuleItem SubModuleItem { get; set; }

        public virtual int? LmsQuizId { get; set; }

        public virtual bool IsPostQuiz { get; set; }

        public virtual int PassingScore { get; set; }

        public virtual Guid Guid { get; set; }

        #endregion

        public Quiz()
        { 
            Results = new List<QuizResult>();
        }

    }

}