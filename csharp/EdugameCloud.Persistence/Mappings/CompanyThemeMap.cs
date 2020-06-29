namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The sub module item theme map.
    /// </summary>
    public class CompanyThemeMap : BaseClassMapGuid<CompanyTheme>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyThemeMap" /> class.
        /// </summary>
        public CompanyThemeMap()
        {
            this.Map(x => x.HeaderBackgroundColor).Length(10).Nullable();
            this.Map(x => x.ButtonColor).Length(10).Nullable();
            this.Map(x => x.ButtonTextColor).Length(10).Nullable();
            this.Map(x => x.GridHeaderTextColor).Length(10).Nullable();
            this.Map(x => x.GridHeaderBackgroundColor).Length(10).Nullable();
            this.Map(x => x.LoginHeaderTextColor).Length(10).Nullable();
            this.Map(x => x.GridRolloverColor).Length(10).Nullable();

            this.Map(x => x.PopupHeaderBackgroundColor).Length(10).Nullable();
            this.Map(x => x.PopupHeaderTextColor).Length(10).Nullable();
            this.Map(x => x.QuestionColor).Length(10).Nullable();
            this.Map(x => x.QuestionHeaderColor).Length(10).Nullable();
            this.Map(x => x.WelcomeTextColor).Length(10).Nullable();

            this.References(x => x.Logo).Nullable().Column("logoId");
        }

        #endregion
    }
}