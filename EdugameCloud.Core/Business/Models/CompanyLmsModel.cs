namespace EdugameCloud.Core.Business.Models
{
    using System.Collections;
    using System.Collections.Generic;

    using DocumentFormat.OpenXml.Spreadsheet;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    public class CompanyLmsModel : BaseModel<CompanyLms, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseModel"/> class.
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
                    .Where(c => c.Company.Id == companyId);
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
        /// Gets one by domain
        /// </summary>
        /// <param name="domain">
        /// The domain
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<CompanyLms> GetOneByDomainOrConsumerKey(string domain, string consumerKey)
        {
            var defaultQuery = new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                .Where(x => (x.LmsDomain != null && x.LmsDomain == domain) || (x.ConsumerKey != null && x.ConsumerKey == consumerKey)).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        #endregion
    }
}