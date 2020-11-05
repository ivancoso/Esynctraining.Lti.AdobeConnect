namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The country DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class CountryDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountryDTO" /> class.
        /// </summary>
        public CountryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryDTO"/> class.
        /// </summary>
        /// <param name="c">
        /// The country.
        /// </param>
        public CountryDTO(Country c)
        {
            this.countryId = c.Id;
            this.country = c.CountryName;
            this.countryCode = c.CountryCode;
            this.countryCode3 = c.CountryCode3;
        }

        #endregion

        #region Public Properties

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
        /// Gets or sets the country code.
        /// </summary>
        [DataMember]
        public string countryCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [DataMember]
        public string countryCode3 { get; set; }

        #endregion
    }
}