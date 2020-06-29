namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The ac session mapping
    /// </summary>
    public class ACSessionMap : BaseClassMap<ACSession>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionMap"/> class.
        /// </summary>
        public ACSessionMap()
        {
            this.Map(x => x.AccountId).Not.Nullable();
            this.Map(x => x.MeetingUrl).Length(500).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.ScoId).Not.Nullable();
            this.Map(x => x.IncludeAcEmails).Nullable();
            this.Map(x => x.Status).CustomType<ACSessionStatusEnum>().Not.Nullable().Default("1");
            this.References(x => x.SubModuleItem).Not.Nullable();
            this.References(x => x.ACUserMode).Not.Nullable();
            this.References(x => x.User).Not.Nullable();
            this.References(x => x.Language).Not.Nullable();

            this.HasMany(x => x.GroupDiscussions).Cascade.Delete().Inverse();
            this.HasMany(x => x.SNMembers).Cascade.Delete().Inverse();
            this.HasMany(x => x.QuizResults).Cascade.Delete().Inverse();
            this.HasMany(x => x.SurveyResults).Cascade.Delete().Inverse();
            this.HasMany(x => x.TestResults).Cascade.Delete().Inverse();
            this.HasMany(x => x.AppletResults).Cascade.Delete().Inverse();
        }

        #endregion
    }
}