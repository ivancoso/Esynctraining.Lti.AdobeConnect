namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The timezone mapping
    /// </summary>
    public class TimeZoneMap : BaseClassMap<TimeZone>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneMap"/> class.
        /// </summary>
        public TimeZoneMap()
        {
            this.Map(x => x.TimeZoneName).Length(50).Not.Nullable();
            this.Map(x => x.TimeZoneGMTDiff).Not.Nullable();
        }

        #endregion
    }
}