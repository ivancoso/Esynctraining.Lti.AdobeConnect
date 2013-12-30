namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The geo country DTO.
    /// </summary>
    [DataContract]
    public class GeoCountryDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeoCountryDTO" /> class.
        /// </summary>
        public GeoCountryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoCountryDTO"/> class.
        /// </summary>
        /// <param name="c">
        /// The country.
        /// </param>
        public GeoCountryDTO(Country c)
        {
            this.countryId = c.Id;
            this.country = c.CountryName;
            this.countryCode = c.CountryCode;
            this.countryCode3 = c.CountryCode3;
            this.latitude = c.Latitude;
            this.longitude = c.Longitude;
            this.zoomLevel = c.ZoomLevel;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [DataMember]
        public decimal latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [DataMember]
        public decimal longitude { get; set; }

        /// <summary>
        /// Gets or sets the zoom level.
        /// </summary>
        [DataMember]
        public int zoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [DataMember]
        public int countryId { get; set; }

        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        [DataMember]
        public string country { get; set; }

        /// <summary>
        /// Gets or sets the country code 3 symbols.
        /// </summary>
        [DataMember]
        public string countryCode3 { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [DataMember]
        public string countryCode { get; set; }

        #endregion
    }
}