namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The subscription dto.
    /// </summary>
    [DataContract]
    public class SubscriptionResDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [DataMember]
        public SubscriptionItem data { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        [DataMember]
        public SubscriptionMeta meta { get; set; }

        /// <summary>
        /// Gets or sets the RAW.
        /// </summary>
        [DataMember]
        public string raw { get; set; }

        #endregion
    }
}