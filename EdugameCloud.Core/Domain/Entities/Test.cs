namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The test
    /// </summary>
    public class Test : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        ///     Gets or sets the instruction description.
        /// </summary>
        public virtual string InstructionDescription { get; set; }

        /// <summary>
        ///     Gets or sets the instruction title.
        /// </summary>
        public virtual string InstructionTitle { get; set; }

        /// <summary>
        ///     Gets or sets the time limit.
        /// </summary>
        public virtual int? TimeLimit { get; set; }

        /// <summary>
        ///     Gets or sets the passing score.
        /// </summary>
        public virtual decimal? PassingScore { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<TestResult> Results { get; protected set; }

        /// <summary>
        ///     Gets or sets the score format.
        /// </summary>
        public virtual string ScoreFormat { get; set; }

        /// <summary>
        ///     Gets or sets the score type.
        /// </summary>
        public virtual ScoreType ScoreType { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        ///     Gets or sets the test name.
        /// </summary>
        public virtual string TestName { get; set; }

        #endregion

        public Test()
        {
            Results = new List<TestResult>();
        }

    }

}