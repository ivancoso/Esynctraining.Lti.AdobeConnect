namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The module mapping
    /// </summary>
    public class ModuleMap : BaseClassMap<Module>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleMap"/> class. 
        /// </summary>
        public ModuleMap()
        {
            this.Map(x => x.ModuleName).Length(50).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.HasMany(x => x.SubModules).ExtraLazyLoad().Cascade.Delete().Inverse();
        }

        #endregion
    }
}