namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz mapping
    /// </summary>
    public class SubscriptionHistoryLogMap : BaseClassMap<SubscriptionHistoryLog>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionHistoryLogMap"/> class. 
        /// </summary>
        public SubscriptionHistoryLogMap()
        {
            this.Map(x => x.SubscriptionId).Not.Nullable();
            this.Map(x => x.SubscriptionTag).Length(500).Not.Nullable();
            this.Map(x => x.LastQueryTime).Nullable();
        }

        #endregion
    }
}