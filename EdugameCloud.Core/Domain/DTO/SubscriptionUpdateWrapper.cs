namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;

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
            this.data = new List<SubscriptionUpdateDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public List<SubscriptionUpdateDTO> data { get; set; }

        #endregion
    }
}