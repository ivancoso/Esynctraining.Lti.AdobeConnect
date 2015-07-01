namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The time zone
    /// </summary>
    public class TimeZone : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the time zone gmt diff.
        /// </summary>
        public virtual float TimeZoneGMTDiff { get; set; }

        /// <summary>
        /// Gets or sets the time zone name.
        /// </summary>
        public virtual string TimeZoneName { get; set; }

        #endregion
    }
}