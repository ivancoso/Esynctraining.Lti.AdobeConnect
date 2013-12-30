namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The ac user mode mapping
    /// </summary>
    public class ACUserModeMap : BaseClassMap<ACUserMode>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACUserModeMap"/> class.
        /// </summary>
        public ACUserModeMap()
        {
            this.Map(x => x.UserMode).Length(50).Nullable();
            this.References(x => x.Image).Column("imageId");
        }

        #endregion
    }
}