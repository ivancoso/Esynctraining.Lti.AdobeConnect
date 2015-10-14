namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class CompanyModel : BaseModel<Company, int>
    {
        public sealed class CompanyCacheItem
        {
            public int CompanyId { get; set; }

            public bool IsActive { get; set; }

            public DateTime? CurrentLicenseExpiryDateUtc { get; set; }

        }

        private readonly ICache _cache;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyModel(IRepository<Company, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        #endregion

        public IEnumerable<CompanyFlatDTO> GetAllFlat()
        {
            var queryOver = new DefaultQueryOver<Company, int>().GetQueryOver()
                .Fetch(x => x.Licenses).Eager
                .TransformUsing(Transformers.DistinctRootEntity);

            var companies = this.Repository.FindAll(queryOver);

            var now = DateTime.Now;
            return companies.Select(CompanyFlatDTO.CreateCompanyFlatDto).ToList();
        }

        public Company GetWithRelated(int companyId)
        {
            var queryOver = new DefaultQueryOver<Company, int>().GetQueryOver()
                .Fetch(x => x.Address).Eager
                .Fetch(x => x.Address.State).Eager
                .Fetch(x => x.Address.Country).Eager
                .Fetch(x => x.PrimaryContact).Eager
                .Fetch(x => x.PrimaryContact.UserRole).Eager
                .Fetch(x => x.Licenses).Eager
                .Fetch(x => x.Theme).Eager
                .Where(x => x.Id == companyId)
                .TransformUsing(Transformers.DistinctRootEntity);

            return this.Repository.FindOne(queryOver).Value;
        }

        // TRICK: use from LTI ONLY!!
        public bool IsActive(int companyId)
        {
            var item = CacheUtility.GetCachedItem<CompanyCacheItem>(_cache, companyId.ToString(), () =>
            {
                var company = GetOneById(companyId).Value;
                var status = company.Status;
                DateTime? expiryDate = null;
                if (company.CurrentLicense != null)
                    expiryDate = company.CurrentLicense.ExpiryDate.ToUniversalTime();

                var result = new CompanyCacheItem
                {
                    CompanyId = company.Id,
                    IsActive = company.Status == CompanyStatus.Active,
                    CurrentLicenseExpiryDateUtc = expiryDate,
                };

                return result;
            });

            return item.IsActive
                && item.CurrentLicenseExpiryDateUtc.HasValue
                && item.CurrentLicenseExpiryDateUtc.Value > DateTime.UtcNow;
        }

        public IEnumerable<Company> GetAllWithRelated()
        {
            var queryOver = new DefaultQueryOver<Company, int>().GetQueryOver()
                .Fetch(x => x.Address).Eager
                .Fetch(x => x.Address.State).Eager
                .Fetch(x => x.Address.Country).Eager
                .Fetch(x => x.PrimaryContact).Eager
                .Fetch(x => x.PrimaryContact.UserRole).Eager
                .Fetch(x => x.Licenses).Eager
                .Fetch(x => x.Theme).Eager
                .TransformUsing(Transformers.DistinctRootEntity);

            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by users.
        /// </summary>
        /// <param name="contactId">
        /// The contact Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IFutureValue<Company> GetOneByPrimaryContact(int contactId)
        {
            var defaultQuery = new QueryOverCompany().GetQueryOver().Where(x => x.PrimaryContact != null && x.PrimaryContact.Id == contactId).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get all by users.
        /// </summary>
        /// <param name="users">
        /// The to list.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> GetAllByUsers(List<int> users)
        {
            User @usr = null;
            var idSubQuery = QueryOver.Of(() => @usr)
                    .WhereRestrictionOn(() => @usr.Id).IsIn(users)
                    .Select(Projections.Distinct(Projections.Property<User>(s => s.Company.Id)));
            var query = new DefaultQueryOver<Company, int>().GetQueryOver()
                .OrderBy(x => x.CompanyName)
                .Asc.WithSubquery.WhereProperty(x => x.Id)
                .In(idSubQuery);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by company theme id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Company}"/>.
        /// </returns>
        public IFutureValue<Company> GetOneByCompanyThemeId(Guid id)
        {
            var defaultQuery = new QueryOverCompany().GetQueryOver().Where(x => x.Theme != null && x.Theme.Id == id).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get all by company theme id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> GetAllByCompanyThemeId(Guid id)
        {
            var defaultQuery = new QueryOverCompany().GetQueryOver().Where(x => x.Theme != null && x.Theme.Id == id);
            return this.Repository.FindAll(defaultQuery);
        }
    }
}