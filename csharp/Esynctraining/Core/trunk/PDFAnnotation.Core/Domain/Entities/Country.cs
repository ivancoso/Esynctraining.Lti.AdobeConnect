namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The country
    /// </summary>
    public class Country : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public virtual string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public virtual string CountryCode3 { get; set; }

        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public virtual string CountryName { get; set; }

        #endregion
    }
}