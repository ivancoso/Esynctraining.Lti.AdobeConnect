namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The test result mapping
    /// </summary>
    public class TestResultMap : BaseClassMap<TestResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultMap"/> class. 
        /// </summary>
        public TestResultMap()
        {
            this.Map(x => x.ParticipantName).Length(200).Not.Nullable();
            this.Map(x => x.Score).Not.Nullable();
            this.Map(x => x.IsArchive).Nullable();
            this.Map(x => x.Email).Nullable();
            this.Map(x => x.ACEmail).Length(500).Nullable();
            this.Map(x => x.StartTime).Not.Nullable();
            this.Map(x => x.EndTime).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.HasMany(x => x.Results).Cascade.Delete().ExtraLazyLoad().Inverse();
            this.References(x => x.Test).Not.Nullable();
            this.Map(x => x.IsCompleted).Nullable();
        }

        #endregion
    }
}