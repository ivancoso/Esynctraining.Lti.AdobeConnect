namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The survey.
    /// </summary>
    public class Survey : Entity
    {
        #region Fields

        /// <summary>
        /// The results.
        /// </summary>
        private ISet<SurveyResult> results = new HashedSet<SurveyResult>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        ///     Gets or sets the survey grouping type.
        /// </summary>
        public virtual SurveyGroupingType SurveyGroupingType { get; set; }

        /// <summary>
        ///     Gets or sets the survey name.
        /// </summary>
        public virtual string SurveyName { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual ISet<SurveyResult> Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        #endregion
    }
}