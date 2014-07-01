namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    
    /// <summary>
    /// The user map.
    /// </summary>
    public class NewsletterSubscriptionMap : BaseClassMap<NewsletterSubscription>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterSubscription"/> class.
        /// </summary>
        public NewsletterSubscriptionMap()
        {
            this.Map(x => x.Email).Length(50).Not.Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
            this.Map(x => x.DateSubscribed).Not.Nullable();
            this.Map(x => x.DateUnsubscribed).Nullable();

        }


        #endregion
    }
}