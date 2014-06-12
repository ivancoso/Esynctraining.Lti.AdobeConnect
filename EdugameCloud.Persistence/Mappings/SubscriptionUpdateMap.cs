namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz mapping
    /// </summary>
    public class SubscriptionUpdateMap : BaseClassMap<SubscriptionUpdate>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateMap"/> class. 
        /// </summary>
        public SubscriptionUpdateMap()
        {
            this.Map(x => x.Changed_aspect).Length(50).Not.Nullable();
            this.Map(x => x.Subscription_id).Not.Nullable();
            this.Map(x => x.ObjectType).Not.Nullable().Column("object").Length(20);
            this.Map(x => x.Object_id).Length(1000).Not.Nullable();
            this.Map(x => x.CreatedDate).Not.Nullable();
            this.Map(x => x.Time).Not.Nullable();
        }

        #endregion
    }
}