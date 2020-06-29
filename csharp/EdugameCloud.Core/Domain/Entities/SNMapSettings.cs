namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN map settings.
    /// </summary>
    public class SNMapSettings : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the map provider.
        /// </summary>
        public virtual SNMapProvider MapProvider { get; set; }

        /// <summary>
        /// Gets or sets a zoom level.
        /// </summary>
        public virtual int? ZoomLevel { get; set; }

        #endregion
    }
}