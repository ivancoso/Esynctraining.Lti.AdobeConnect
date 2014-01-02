namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;

    using NHibernate;
    using NHibernate.Criterion;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Company model.
    /// </summary>
    public class CompanyModel : BaseModel<Company, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyModel(IRepository<Company, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> Search(string name)
        {
            QueryOver<Company, Company> defaultQuery =
                new DefaultQueryOver<Company, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.CompanyName)
                    .IsInsensitiveLike("%" + name + "%");
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by company id.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<int> GetAllIds()
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().Select(x => x.Id);
            return this.Repository.FindAll<int>(query).ToList().Distinct();
        }

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="companyName">
        /// The company name.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Company}"/>.
        /// </returns>
        public IFutureValue<Company> GetOneByName(string companyName)
        {
            var companyNameToLower = companyName.Return(x => x.ToLowerInvariant(), string.Empty);
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().WhereRestrictionOn(x => x.CompanyName).IsInsensitiveLike(companyNameToLower);
            return this.Repository.FindOne(query);
        }

        #endregion
    }
}