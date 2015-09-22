namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The subscription updatex
    /// </summary>
    public class SubscriptionUpdate : Entity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdate"/> class.
        /// </summary>
        public SubscriptionUpdate()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.CreatedDate = DateTime.Now;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the changed aspect.
        /// </summary>
        public virtual string Changed_aspect { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public virtual string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        public virtual string Object_id { get; set; }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        public virtual int Subscription_id { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public virtual string Time { get; set; }

        #endregion
    }
}

