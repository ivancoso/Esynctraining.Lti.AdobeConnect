namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The SN service map.
    /// </summary>
    public class SNServiceMap : BaseClassMap<SNService>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNServiceMap"/> class.
        /// </summary>
        public SNServiceMap()
        {
            this.Map(x => x.SocialService).Length(255).Not.Nullable();
        }

        #endregion
    }
}