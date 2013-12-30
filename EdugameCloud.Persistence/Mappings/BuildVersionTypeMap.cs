namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The build version type mapping
    /// </summary>
    public class BuildVersionTypeMap : BaseClassMap<BuildVersionType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionTypeMap"/> class.
        /// </summary>
        public BuildVersionTypeMap()
        {
            this.Map(x => x.BuildVersionTypeName).Not.Nullable();
        }

        #endregion
    }
}