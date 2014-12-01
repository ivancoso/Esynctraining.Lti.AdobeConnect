namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The sub module category mapping
    /// </summary>
    public class SubModuleCategoryMap : BaseClassMap<SubModuleCategory>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleCategoryMap"/> class. 
        /// </summary>
        public SubModuleCategoryMap()
        {
            this.Map(x => x.CategoryName).Length(255).Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.LmsCourseId).Nullable();

            this.HasMany(x => x.SubModuleItems).ExtraLazyLoad().Cascade.Delete().Inverse();

            this.References(x => x.User);
            this.References(x => x.SubModule);
            this.References(x => x.CompanyLms).Nullable();
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<SubModuleCategory>(x => x.ModifiedBy)));
        }

        #endregion
    }
}