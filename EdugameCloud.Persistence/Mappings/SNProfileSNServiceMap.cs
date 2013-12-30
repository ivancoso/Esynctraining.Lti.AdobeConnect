namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The SN profile SN service map.
    /// </summary>
    public class SNProfileSNServiceMap : BaseClassMap<SNProfileSNService>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileSNServiceMap"/> class.
        /// </summary>
        public SNProfileSNServiceMap()
        {
            this.Map(x => x.IsEnabled).Not.Nullable();
            this.Map(x => x.ServiceUrl).Length(2000).Nullable();
            this.References(x => x.Profile).Not.Nullable();
            this.References(x => x.Service).Not.Nullable();
        }

        #endregion
    }
}