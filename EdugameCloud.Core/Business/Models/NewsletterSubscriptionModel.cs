namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    public class NewsletterSubscriptionModel : BaseModel<NewsletterSubscription, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterSubscriptionModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public NewsletterSubscriptionModel(IRepository<NewsletterSubscription, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{NewsletterSubscription}"/>.
        /// </returns>
        public override IEnumerable<NewsletterSubscription> GetAll()
        {
            var query = new DefaultQueryOver<NewsletterSubscription, int>().GetQueryOver();
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{NewsletterSubscription}"/>.
        /// </returns>
        public IFutureValue<NewsletterSubscription> GetOneByEmail(string email)
        {
            var queryOver = new DefaultQueryOver<NewsletterSubscription, int>().GetQueryOver().Where(x => x.Email == email);
            return this.Repository.FindOne(queryOver);
        }
    }
}
