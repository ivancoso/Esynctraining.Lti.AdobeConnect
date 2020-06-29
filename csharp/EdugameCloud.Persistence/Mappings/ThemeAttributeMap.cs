namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The theme mapping
    /// </summary>
    public class ThemeAttributeMap : BaseClassMap<ThemeAttribute>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeAttributeMap"/> class.
        /// </summary>
        public ThemeAttributeMap()
        {
            this.References(x => x.Theme).Not.Nullable();
            this.Map(x => x.ThemeOrder).Nullable();
            this.Map(x => x.BgColor).Length(6).Nullable();
            this.Map(x => x.TitleColor).Length(6).Nullable();
            this.Map(x => x.CategoryColor).Length(6).Nullable();
            this.Map(x => x.SelectionColor).Length(6).Nullable();
            this.Map(x => x.QuestionHintColor).Length(6).Nullable();
            this.Map(x => x.QuestionTextColor).Length(6).Nullable();
            this.Map(x => x.QuestionInstructionColor).Length(6).Nullable();
            this.Map(x => x.ResponseCorrectColor).Length(6).Nullable();
            this.Map(x => x.ResponseIncorrectColor).Length(6).Nullable();
            this.Map(x => x.DistractorTextColor).Length(6).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Theme>(x => x.CreatedBy)));
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Theme>(x => x.ModifiedBy)));
        }

        #endregion
    }
}