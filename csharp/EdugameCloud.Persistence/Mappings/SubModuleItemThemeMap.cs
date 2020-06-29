namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The sub module item theme map.
    /// </summary>
    public class SubModuleItemThemeMap : BaseClassMap<SubModuleItemTheme>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubModuleItemThemeMap" /> class.
        /// </summary>
        public SubModuleItemThemeMap()
        {
            this.Map(x => x.BackgroundColor).Length(10).Nullable().Column("bgColor");
            this.Map(x => x.TitleColor).Length(10).Nullable();
            this.Map(x => x.QuestionColor).Length(10).Nullable();
            this.Map(x => x.InstructionColor).Length(10).Nullable();
            this.Map(x => x.CorrectColor).Length(10).Nullable();
            this.Map(x => x.IncorrectColor).Length(10).Nullable();
            this.Map(x => x.SelectionColor).Length(10).Nullable();
            this.Map(x => x.HintColor).Length(10).Nullable();

            this.References(x => x.SubModuleItem).Not.Nullable();
            this.References(x => x.BackgroundImage).Nullable().Column("bgImageId");
        }

        #endregion
    }
}