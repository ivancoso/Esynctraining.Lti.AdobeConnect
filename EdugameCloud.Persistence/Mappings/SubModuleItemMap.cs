namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The sub module item mapping
    /// </summary>
    public class SubModuleItemMap : BaseClassMap<SubModuleItem>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemMap"/> class. 
        /// </summary>
        public SubModuleItemMap()
        {
            this.Map(x => x.IsActive).Nullable();
            this.Map(x => x.IsShared).Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();

            this.HasMany(x => x.AppletItems).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Questions).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Quizes).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Surveys).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Tests).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.SNProfiles).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.ACSessions).ExtraLazyLoad().Cascade.Delete().Inverse();

            this.References(x => x.SubModuleCategory);
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<SubModuleItem>(x => x.ModifiedBy)));
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<SubModuleItem>(x => x.CreatedBy)));
        }

        #endregion
    }
}