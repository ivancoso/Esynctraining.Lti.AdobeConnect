namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The survey grouping type mapping
    /// </summary>
    public class SurveyGroupingTypeMap : BaseClassMap<SurveyGroupingType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyGroupingTypeMap"/> class. 
        /// </summary>
        public SurveyGroupingTypeMap()
        {
            this.Map(x => x.SurveyGroupingTypeName).Length(255).Not.Nullable();
        }

        #endregion
    }
}