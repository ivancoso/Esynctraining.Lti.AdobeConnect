namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN map provider.
    /// </summary>
    [DataContract]
    public class SNMapProviderDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapProviderDTO"/> class.
        /// </summary>
        public SNMapProviderDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapProviderDTO"/> class.
        /// </summary>
        /// <param name="mapProvider">
        /// The map provider.
        /// </param>
        public SNMapProviderDTO(SNMapProvider mapProvider)
        {
            this.mapProvider = mapProvider.MapProvider;
            this.snMapProviderId = mapProvider.Id;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the map provider.
        /// </summary>
        [DataMember]
        public string mapProvider { get; set; }

        /// <summary>
        /// Gets or sets the SN map provider id.
        /// </summary>
        [DataMember]
        public int snMapProviderId { get; set; }

        #endregion
    }
}