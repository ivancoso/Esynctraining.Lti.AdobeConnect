namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The subscription update sum DTO.
    /// </summary>
    [DataContract]
    public class SubscriptionUpdateSumDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateSumDTO"/> class.
        /// </summary>
        public SubscriptionUpdateSumDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateSumDTO"/> class.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="updates">
        /// The updates.
        /// </param>
        /// <param name="latestTime">
        /// The latest Time.
        /// </param>
        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public SubscriptionUpdateSumDTO(string tag, List<SubscriptionUpdate> updates, long latestTime)
        {
            var times = updates.Select(x => long.Parse(x.Time)).ToList();
            var higherTimes = times.Where(x => x >= latestTime).ToList();
            this.object_id = tag;
            this.latestTime = higherTimes.Any() ? higherTimes.Max() : 0L;
            this.itemsCount = higherTimes.Count;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [DataMember]
        public long itemsCount { get; set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        [DataMember]
        public long latestTime { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        [DataMember]
        public string object_id { get; set; }

        #endregion
    }
}