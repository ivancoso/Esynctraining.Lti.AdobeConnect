namespace PDFAnnotation.Core.Business.Models
{
    using System;
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

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="organizationId">
        /// The organization Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Company}"/>.
        /// </returns>
        public IFutureValue<Company> GetOneByOrganizationId(Guid organizationId)
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().Where(x => x.OrganizationId == organizationId).Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get all for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> GetAllForUser(int userId)
        {
            CompanyContact companyContact = null;
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().JoinQueryOver(x => x.CompanyContacts, () => companyContact).Where(() => companyContact.Contact.Id == userId);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="organizationIds">
        /// The organization Ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> GetAllByOrganizationIds(List<Guid> organizationIds)
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().WhereRestrictionOn(x => x.OrganizationId).IsIn(organizationIds);
            return this.Repository.FindAll(query);
        }

        #endregion
    }
}