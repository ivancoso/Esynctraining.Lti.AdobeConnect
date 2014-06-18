namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The subscription history log
    /// </summary>
    public class SubscriptionHistoryLog : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the last query time.
        /// </summary>
        public virtual DateTime? LastQueryTime { get; set; }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        public virtual int SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the subscription tag.
        /// </summary>
        public virtual string SubscriptionTag { get; set; }

        #endregion
    }
}