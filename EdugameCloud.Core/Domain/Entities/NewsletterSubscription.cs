using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class NewsletterSubscription : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the date subscribed.
        /// </summary>
        public virtual DateTime DateSubscribed { get; set; }

        /// <summary>
        /// Gets or sets the dateunsubscribed.
        /// </summary>
        public virtual DateTime? DateUnsubscribed { get; set; }

        /// <summary>
        /// Gets or sets the active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        #endregion
    }
}
