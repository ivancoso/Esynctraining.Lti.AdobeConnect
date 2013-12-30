namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The survey result mapping
    /// </summary>
    public class SurveyResultMap : BaseClassMap<SurveyResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyResultMap"/> class. 
        /// </summary>
        public SurveyResultMap()
        {
            this.Map(x => x.ParticipantName).Length(200).Not.Nullable();
            this.Map(x => x.Score).Not.Nullable();
            this.Map(x => x.StartTime).Not.Nullable();
            this.Map(x => x.EndTime).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.Map(x => x.IsArchive).Nullable();
            this.Map(x => x.Email).Nullable();
            this.HasMany(x => x.Results).Cascade.Delete().ExtraLazyLoad().Inverse();
            this.References(x => x.Survey).Not.Nullable();
        }

        #endregion
    }
}