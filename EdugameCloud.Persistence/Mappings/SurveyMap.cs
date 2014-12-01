namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The survey mapping
    /// </summary>
    public class SurveyMap : BaseClassMap<Survey>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyMap"/> class. 
        /// </summary>
        public SurveyMap()
        {
            this.Map(x => x.SurveyName).Length(255).Not.Nullable();
            this.Map(x => x.Description).Nullable();

            this.HasMany(x => x.Results).ExtraLazyLoad().Cascade.Delete().Inverse();

            this.References(x => x.SubModuleItem).Nullable();
            this.References(x => x.SurveyGroupingType).Not.Nullable();

            this.Map(x => x.LmsSurveyId).Nullable();
        }

        #endregion
    }
}