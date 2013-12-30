namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The survey grouping type.
    /// </summary>
    public class SurveyGroupingType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the survey grouping type.
        /// </summary>
        public virtual string SurveyGroupingTypeName { get; set; }

        #endregion
    }
}