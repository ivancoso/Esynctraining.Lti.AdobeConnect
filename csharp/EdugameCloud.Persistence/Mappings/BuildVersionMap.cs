namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The build version mapping
    /// </summary>
    public class BuildVersionMap : BaseClassMap<BuildVersion>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionMap"/> class.
        /// </summary>
        public BuildVersionMap()
        {
            this.Map(x => x.BuildNumber).Length(20).Not.Nullable();
            this.Map(x => x.DescriptionHTML).Nullable();
            this.Map(x => x.DescriptionSmall).Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
            this.References(x => x.Type).Not.Nullable();
            this.References(x => x.File).Nullable();
        }

        #endregion
    }
}