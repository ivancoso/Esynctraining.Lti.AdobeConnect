namespace EdugameCloud.Core.Domain.DTO
{
    /// <summary>
    /// The subscription update wrapper.
    /// </summary>
    public class SubscriptionUpdateWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateWrapper"/> class.
        /// </summary>
        public SubscriptionUpdateWrapper()
        {
            this.data = new SubscriptionUpdateDTO[] { };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public SubscriptionUpdateDTO[] data { get; set; }

        #endregion
    }
}