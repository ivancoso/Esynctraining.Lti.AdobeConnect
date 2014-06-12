namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    /// <summary>
    ///     The SubscriptionUpdate model.
    /// </summary>
    public class SubscriptionUpdateModel : BaseModel<SubscriptionUpdate, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUpdateModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SubscriptionUpdateModel(IRepository<SubscriptionUpdate, int> repository)
            : base(repository)
        {
        }

        #endregion

        public IEnumerable<SubscriptionUpdate> GetAllByTag(string tag)
        {
            var query =
                new DefaultQueryOver<SubscriptionUpdate, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Object_id)
                    .IsInsensitiveLike(tag);
            return this.Repository.FindAll(query);
        }
    }
}