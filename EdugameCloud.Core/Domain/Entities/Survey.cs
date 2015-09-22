namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The survey.
    /// </summary>
    public class Survey : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the survey grouping type.
        /// </summary>
        public virtual SurveyGroupingType SurveyGroupingType { get; set; }

        /// <summary>
        /// Gets or sets the survey name.
        /// </summary>
        public virtual string SurveyName { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<SurveyResult> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        /// Gets or sets the lms id.
        /// </summary>
        public virtual int? LmsSurveyId { get; set; }

        #endregion

        public Survey()
        {
            Results = new List<SurveyResult>();
        }

    }

}