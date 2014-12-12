namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz result mapping
    /// </summary>
    public class QuizResultMap : BaseClassMap<QuizResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultMap"/> class. 
        /// </summary>
        public QuizResultMap()
        {
            this.Map(x => x.ParticipantName).Length(200).Not.Nullable();
            this.Map(x => x.Score).Not.Nullable();
            this.Map(x => x.StartTime).Not.Nullable();
            this.Map(x => x.EndTime).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.Map(x => x.IsArchive).Nullable();
            this.Map(x => x.Email).Length(500).Nullable();
            this.Map(x => x.ACEmail).Length(500).Nullable();
            this.Map(x => x.LmsId).Nullable();
            this.Map(x => x.isCompleted).Nullable();
            this.Map(x => x.LmsUserParametersId).Nullable();

            this.HasMany(x => x.Results).Cascade.Delete().ExtraLazyLoad().Inverse();
            this.References(x => x.Quiz).Not.Nullable();
        }

        #endregion
    }
}