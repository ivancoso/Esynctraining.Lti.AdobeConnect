namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using TimeZone = EdugameCloud.Core.Domain.Entities.TimeZone;

    /// <summary>
    ///     The time zone dto.
    /// </summary>
    [DataContract]
    public class TimeZoneDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneDTO"/> class.
        /// </summary>
        public TimeZoneDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public TimeZoneDTO(TimeZone result)
        {
            this.timeZoneId = result.Id;
            this.timeZone = result.TimeZoneName;
            this.timeZoneGMTDiff = result.TimeZoneGMTDiff;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the time zone GMT difference.
        /// </summary>
        [DataMember]
        public virtual float? timeZoneGMTDiff { get; set; }

        /// <summary>
        ///     Gets or sets the time zone.
        /// </summary>
        [DataMember]
        public virtual string timeZone { get; set; }

        /// <summary>
        /// Gets or sets the time zone id.
        /// </summary>
        [DataMember]
        public virtual int timeZoneId { get; set; }

        #endregion
    }
}