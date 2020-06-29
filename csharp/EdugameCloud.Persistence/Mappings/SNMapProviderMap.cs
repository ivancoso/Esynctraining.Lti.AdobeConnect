namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN Map provider map.
    /// </summary>
    public class SNMapProviderMap : BaseClassMap<SNMapProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapProviderMap"/> class. 
        /// </summary>
        public SNMapProviderMap()
        {
            this.Map(x => x.MapProvider).Length(255).Not.Nullable();
        }
    }
}