namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The newsletter subscription DTO.
    /// </summary>
    [DataContract]
    public class NewsletterSubscriptionDTO
    {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NewsletterSubscriptionDTO" /> class.
            /// </summary>
            public NewsletterSubscriptionDTO()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NewsletterSubscriptionDTO"/> class.
            /// </summary>
            /// <param name="subscription">
            /// The subscription.
            /// </param>
            public NewsletterSubscriptionDTO(NewsletterSubscription subscription)
            {
                this.newsletterSubscriptionId = subscription.Id;
                this.email = subscription.Email;
                this.isActive = subscription.IsActive;
                this.dateSubscribed = subscription.DateSubscribed.With(x => x.ConvertToUnixTimestamp());
                this.dateUnsubscribed = (subscription.DateUnsubscribed ?? DateTime.Today.AddYears(1)).With(x => x.ConvertToUnixTimestamp());
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the newsletter subscription id.
            /// </summary>
            [DataMember]
            public int newsletterSubscriptionId { get; set; }

            /// <summary>
            /// Gets or sets the company id.
            /// </summary>
            [DataMember]
            public string email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
            public bool isActive { get; set; }

            /// <summary>
            /// Gets or sets the date created.
            /// </summary>
            [DataMember]
            public double dateSubscribed { get; set; }

            /// <summary>
            /// Gets or sets the date unsubscribed.
            /// </summary>
            [DataMember]
            public double dateUnsubscribed { get; set; }

            #endregion
    }
}
