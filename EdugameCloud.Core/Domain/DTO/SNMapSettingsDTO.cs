namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The SN map settings.
    /// </summary>
    [DataContract]
    [KnownType(typeof(AddressDTO))]
    public class SNMapSettingsDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapSettingsDTO"/> class.
        /// </summary>
        public SNMapSettingsDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapSettingsDTO"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public SNMapSettingsDTO(SNMapSettings settings)
        {
            if (settings != null)
            {
                this.snMapSettingsId = settings.Id;
                this.snMapProviderId = settings.MapProvider.Return(x => x.Id, (int?)null);
                this.countryId = settings.Country.Return(x => x.Id, (int?)null);
                this.zoomLevel = settings.ZoomLevel;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the country.
        /// </summary>
        [DataMember]
        public int? countryId { get; set; }

        /// <summary>
        ///     Gets or sets the map provider.
        /// </summary>
        [DataMember]
        public int? snMapProviderId { get; set; }

        /// <summary>
        /// Gets or sets the SN map settings id.
        /// </summary>
        [DataMember]
        public int snMapSettingsId { get; set; }

        /// <summary>
        ///     Gets or sets a zoom level.
        /// </summary>
        [DataMember]
        public int? zoomLevel { get; set; }

        #endregion
    }
}