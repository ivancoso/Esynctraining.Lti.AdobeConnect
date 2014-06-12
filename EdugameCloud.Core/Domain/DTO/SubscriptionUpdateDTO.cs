namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The subscription update DTO.
    /// </summary>
    [DataContract]
    public class SubscriptionUpdateDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateDTO"/> class.
        /// </summary>
        public SubscriptionUpdateDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateDTO"/> class.
        /// </summary>
        /// <param name="su">
        /// The su.
        /// </param>
        public SubscriptionUpdateDTO(SubscriptionUpdate su)
        {
            this.subscription_id = su.Subscription_id;
            this.changed_aspect = su.Changed_aspect;
            this.@object = su.ObjectType;
            this.object_id = su.Object_id;
            this.time = su.Time;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the changed aspect.
        /// </summary>
        [DataMember]
        public string changed_aspect { get; set; }

        /// <summary>
        ///     Gets or sets the object type.
        /// </summary>
        [DataMember]
        public string @object { get; set; }

        /// <summary>
        ///     Gets or sets the object id.
        /// </summary>
        [DataMember]
        public string object_id { get; set; }

        /// <summary>
        ///     Gets or sets the subscription id.
        /// </summary>
        [DataMember]
        public int subscription_id { get; set; }

        /// <summary>
        ///     Gets or sets the time.
        /// </summary>
        [DataMember]
        public string time { get; set; }

        #endregion

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("object_id={0}; time={1}; object_type={2}", this.object_id, this.time, this.@object);
        }
    }
}