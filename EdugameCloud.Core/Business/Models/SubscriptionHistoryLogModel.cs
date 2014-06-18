namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;

    using NHibernate;

    /// <summary>
    ///     The SubscriptionUpdate model.
    /// </summary>
    public class SubscriptionHistoryLogModel : BaseModel<SubscriptionHistoryLog, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionHistoryLogModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SubscriptionHistoryLogModel(IRepository<SubscriptionHistoryLog, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{SubscriptionHistoryLog}"/>.
        /// </returns>
        public IFutureValue<SubscriptionHistoryLog> GetOneByTag(string tag)
        {
            var query =
                new DefaultQueryOver<SubscriptionHistoryLog, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.SubscriptionTag)
                    .IsInsensitiveLike(tag).Take(1);
            return this.Repository.FindOne(query);
        }

        public IEnumerable<SubscriptionHistoryLog> GetAllByTags(List<string> tags)
        {
            var query =
                new DefaultQueryOver<SubscriptionHistoryLog, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.SubscriptionTag)
                    .IsIn(tags);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="tagRequestDtos">
        /// The tag request dtos.
        /// </param>
        public void SaveUpdate(List<Tuple<string, int>> tagRequestDtos)
        {
            var tags = tagRequestDtos.Select(x => x.Item1).ToList();
            var items = this.GetAllByTags(tags).ToList();
            foreach (var tag in tags)
            {
                var item =
                    items.FirstOrDefault(
                        x => x.SubscriptionTag.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                var subscriptionId = tagRequestDtos.FirstOrDefault(x => x.Item1.Equals(tag, StringComparison.InvariantCultureIgnoreCase)).Return(y => y.Item2, 0);
                if (item != null)
                {
                    item.LastQueryTime = DateTime.Now;
                    if (item.SubscriptionId == 0)
                    {
                        item.SubscriptionId = subscriptionId;
                    }
                }
                else
                {
                    item = new SubscriptionHistoryLog
                               {
                                   SubscriptionTag = tag,
                                   SubscriptionId = subscriptionId,
                                   LastQueryTime = DateTime.Now
                               };
                }

                this.RegisterSave(item);
            }
        }
    }
}