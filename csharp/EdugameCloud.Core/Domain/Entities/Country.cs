namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The country
    /// </summary>
    public class Country : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the country code 2 symbols.
        /// </summary>
        public virtual string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country code 3 symbols.
        /// </summary>
        public virtual string CountryCode3 { get; set; }

        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public virtual string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public virtual decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public virtual int ZoomLevel { get; set; }

        #endregion
    }
}