namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The subscription dto.
    /// </summary>
    [DataContract]
    public class SubscriptionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDTO"/> class.
        /// </summary>
        public SubscriptionDTO()
        {
            this.data = new List<SubscriptionItem>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [DataMember]
        public List<SubscriptionItem> data { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        [DataMember]
        public SubscriptionMeta meta { get; set; }

        #endregion
    }

    /// <summary>
    /// The subscription meta.
    /// </summary>
    [DataContract]
    public class SubscriptionMeta
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [DataMember]
        public int code { get; set; }

        #endregion
    }

    /// <summary>
    /// The subscription item.
    /// </summary>
    [DataContract]
    public class SubscriptionItem
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the aspect.
        /// </summary>
        [DataMember]
        public string aspect { get; set; }

        /// <summary>
        /// Gets or sets the callback_url.
        /// </summary>
        [DataMember]
        public string callback_url { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        [DataMember]
        public string @object { get; set; }

        /// <summary>
        /// Gets or sets the object_id.
        /// </summary>
        [DataMember]
        public string object_id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        #endregion
    }
}