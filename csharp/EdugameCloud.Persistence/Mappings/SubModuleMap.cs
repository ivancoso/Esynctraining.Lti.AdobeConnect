namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The sub module mapping
    /// </summary>
    public class SubModuleMap : BaseClassMap<SubModule>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleMap"/> class. 
        /// </summary>
        public SubModuleMap()
        {
            this.Map(x => x.SubModuleName).Length(50).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.HasMany(x => x.SubModuleCategories).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.References(x => x.Module);
        }

        #endregion
    }
}