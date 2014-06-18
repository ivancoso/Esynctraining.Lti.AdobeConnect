namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
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

        public IEnumerable<SubscriptionUpdate> GetAllBySubscriptionId(string sId)
        {
            var query =
                new DefaultQueryOver<SubscriptionUpdate, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Subscription_id)
                    .IsInsensitiveLike(sId);
            return this.Repository.FindAll(query);
        }

        public IEnumerable<SubscriptionUpdate> GetAllByTag(string tag)
        {
            var query =
                new DefaultQueryOver<SubscriptionUpdate, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Object_id)
                    .IsInsensitiveLike(tag);
            return this.Repository.FindAll(query);
        }

        public IEnumerable<SubscriptionUpdate> GetAllByTags(List<TagRequestDTO> tagList)
        {
            var tags = tagList.Select(x => x.tag).ToList();
            var query =
                new DefaultQueryOver<SubscriptionUpdate, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Object_id)
                    .IsIn(tags);

            return this.Repository.FindAll(query);
        }
    }
}