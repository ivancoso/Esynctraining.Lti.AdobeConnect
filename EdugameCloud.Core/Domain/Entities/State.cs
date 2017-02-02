namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The state
    /// </summary>
    public class State : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        public virtual string StateName { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        public virtual string StateCode { get; set; }

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

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        #endregion

    }
}