namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

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
            /// Initializes a new instance of the <see cref="CompanyDTO"/> class.
            /// </summary>
            public NewsletterSubscriptionDTO(NewsletterSubscription n)
            {
                this.newsletterSubscriptionId = n.Id;
                this.email = n.Email;
                this.isActive = n.IsActive;
                this.dateSubscribed = n.DateSubscribed;
                this.dateUnsubscribed = n.DateUnsubscribed;
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
            /// Gets or sets the active.
            /// </summary>
            [DataMember]
            public bool isActive { get; set; }

            /// <summary>
            /// Gets or sets the date created.
            /// </summary>
            [DataMember]
            public DateTime dateSubscribed { get; set; }

            /// <summary>
            /// Gets or sets the date unsubscribed.
            /// </summary>
            [DataMember]
            public DateTime? dateUnsubscribed { get; set; }

            #endregion
    }
}
