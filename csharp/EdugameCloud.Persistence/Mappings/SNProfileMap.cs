namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The SN profile map.
    /// </summary>
    public class SNProfileMap : BaseClassMap<SNProfile>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileMap"/> class.
        /// </summary>
        public SNProfileMap()
        {
            this.References(x => x.Address).Nullable().Cascade.Delete();
            this.References(x => x.SubModuleItem).Not.Nullable();
            this.References(x => x.MapSettings).Nullable().Cascade.Delete();
            this.Map(x => x.About).Nullable();
            this.Map(x => x.Email).Length(255).Nullable();
            this.Map(x => x.Phone).Length(255).Nullable();
            this.Map(x => x.ProfileName).Length(255).Not.Nullable();
            this.Map(x => x.UserName).Length(255).Not.Nullable();
            this.Map(x => x.JobTitle).Length(500).Nullable();
            this.HasMany(x => x.Links).Cascade.Delete().Inverse();
            this.HasMany(x => x.Services).Cascade.Delete().Inverse();
        }

        #endregion
    }
}