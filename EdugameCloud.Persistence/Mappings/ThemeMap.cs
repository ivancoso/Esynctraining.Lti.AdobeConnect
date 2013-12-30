namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The theme mapping
    /// </summary>
    public class ThemeMap : BaseClassMap<Theme>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeMap"/> class.
        /// </summary>
        public ThemeMap()
        {
            this.Map(x => x.ThemeName).Length(50).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Theme>(x => x.CreatedBy)));
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Theme>(x => x.ModifiedBy)));
        }

        #endregion
    }
}