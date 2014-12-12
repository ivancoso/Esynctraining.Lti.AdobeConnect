namespace EdugameCloud.Lti.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The company LMS model.
    /// </summary>
    public class CompanyLmsModel : BaseModel<CompanyLms, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyLmsModel(IRepository<CompanyLms, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get one by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{CompanyLms}"/>.
        /// </returns>
        public IEnumerable<CompanyLms> GetAllByCompanyId(int companyId)
        {
            var queryOver =
                new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                    .Where(c => c.CompanyId == companyId);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// Gets one by domain
        /// </summary>
        /// <param name="domain">
        /// The domain
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<CompanyLms> GetOneByDomain(string domain)
        {
            var defaultQuery = new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                .Where(x => x.LmsDomain == domain).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get one by ac domain.
        /// </summary>
        /// <param name="adobeConnectDomain">
        /// The ac domain.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{CompanyLms}"/>.
        /// </returns>
        public IFutureValue<CompanyLms> GetOneByAcDomain(string adobeConnectDomain)
        {
            if (adobeConnectDomain.EndsWith("/"))
            {
                adobeConnectDomain = adobeConnectDomain.Remove(adobeConnectDomain.Length - 1);
            }
            var defaultQuery = new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                .WhereRestrictionOn(x => x.AcServer).IsInsensitiveLike(adobeConnectDomain, MatchMode.Start)
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get one by provider and domain or consumer key.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <returns>
        /// The canvas AC meeting
        /// </returns>
        public IFutureValue<CompanyLms> GetOneByProviderAndConsumerKey(string providerName, string consumerKey)
        {
            var defaultQuery = new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                .Where(x => (x.ConsumerKey != null && x.ConsumerKey == consumerKey))
                .JoinQueryOver(x => x.LmsProvider).WhereRestrictionOn(x => x.ShortName).IsInsensitiveLike(providerName)
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get one by provider.
        /// </summary>
        /// <param name="providerId">
        /// The provider Id.
        /// </param>
        /// <returns>
        /// The canvas AC meeting
        /// </returns>
        public IEnumerable<CompanyLms> GetAllByProviderId(int providerId)
        {
            var defaultQuery = new DefaultQueryOver<CompanyLms, int>().GetQueryOver().Where(x => x.LmsProvider.Id == providerId);
            return this.Repository.FindAll(defaultQuery);
        }


        #endregion
    }
}