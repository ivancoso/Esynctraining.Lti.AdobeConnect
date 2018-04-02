namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.FullText;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;
    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class CategoryModel : BaseModel<Category, int>
    {
        private readonly FullTextModel _fullTextModel;

        #region Constructors and Destructors

        public CategoryModel(IRepository<Category, int> repository, FullTextModel fullTextModel)
            : base(repository)
        {
            _fullTextModel = fullTextModel ?? throw new ArgumentNullException(nameof(fullTextModel));
        }

        #endregion

        #region Public Methods and Operators

        public override IEnumerable<Category> GetAll()
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver();
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<int> GetAllIdsByCompanyId(int companyId)
        {
            var query = new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.CompanyId == companyId).Select(x => x.Id);

            return this.Repository.FindAll<int>(query).ToList();
        }

        public IEnumerable<Category> GetAllByCompanyId(int firmId)
        {
            QueryOver<Category, Category> query =
                new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.CompanyId == firmId);
            return this.Repository.FindAll(query).ToList();
        }

#if CONTACTS_DEFINED

        /// <summary>
        /// The get all by company id. and contact
        /// </summary>
        /// <param name="companyIds">
        /// The company Ids.
        /// </param>
        /// <param name="contact">
        /// The contact.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Category}"/>.
        /// </returns>
        public IEnumerable<Category> GetAllByCompanyIdsAndContact(List<int> companyIds, Contact contact)
        {
            var query = new DefaultQueryOver<Category, int>().GetQueryOver().WhereRestrictionOn(x => x.CompanyId).IsIn(companyIds);
            
            if (!contact.IsSuperAdmin && contact.CompanyContacts.ToList().Any(x => x.ContactType.Id == (int)ContactTypeEnum.Contact && companyIds.Contains(x.Company.Id)))
            {
                Contact c = null;
                var contactId = contact.Id;

                var resQuery = query.JoinQueryOver(x => x.Contacts, () => c).Where(() => c.Id == contactId);
                return this.Repository.FindAll(resQuery).ToList();
            }

            return this.Repository.FindAll(query).ToList();
        }

#endif

        public IEnumerable<Category> GetAllByCompanyIds(List<int> companyIds)
        {
            var query = new DefaultQueryOver<Category, int>().GetQueryOver().WhereRestrictionOn(x => x.CompanyId).IsIn(companyIds);
            return this.Repository.FindAll(query).ToList();
        }

        public IEnumerable<Category> GetAllByCompanyAndNamePaged(string searchPattern, int companyId, int pageIndex, int pageSize, out int totalCount)
        {
            var searchIds = new List<int>();
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<Category, int>().GetQueryOver();
            if (companyId != 0)
            {
                queryOver = queryOver.Where(x => x.CompanyId == companyId);
            }

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this._fullTextModel.Search(searchPattern, typeof(Category), int.MaxValue).ToList();
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(searchIds);
            }

            QueryOver<Category, Category> rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            QueryOver<Category> pagedQuery = queryOver.OrderBy(x => x.CategoryName).Asc;

            if (pageSize > 0)
            {
                pagedQuery = pagedQuery.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            }

            return searchIds.Any() ? this.Repository.FindAll(pagedQuery).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(pagedQuery);
        }

        public override IEnumerable<Category> GetAllByIds(List<int> ids)
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Id)
                    .IsIn(ids);
            return this.Repository.FindAll(defaultQuery);
        }

        public override IFutureValue<Category> GetOneById(int id)
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.Id == id);
            return this.Repository.FindOne(defaultQuery);
        }

        public IEnumerable<Category> Search(string name)
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.CategoryName)
                    .IsInsensitiveLike("%" + name + "%");
            return this.Repository.FindAll(defaultQuery);
        }

#endregion

    }

}