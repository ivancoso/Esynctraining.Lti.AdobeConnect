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
    using NHibernate.Transform;

    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Category model class.
    /// </summary>
    public class CategoryModel : BaseModel<Category, int>
    {
        #region Fields

        private readonly FullTextModel fullTextModel;

        /// <summary>
        ///     The contact repository.
        /// </summary>
        private readonly IRepository<Contact, int> contactRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="contactRepository">
        /// The contact Repository.
        /// </param>
        public CategoryModel(IRepository<Category, int> repository, FullTextModel fullTextModel, IRepository<Contact, int> contactRepository)
            : base(repository)
        {
            this.fullTextModel = fullTextModel;
            this.contactRepository = contactRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable{Category}" />.
        /// </returns>
        public override IEnumerable<Category> GetAll()
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver().Fetch(x => x.Company).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Category}"/>.
        /// </returns>
        public IEnumerable<int> GetAllIdsByCompanyId(int companyId)
        {
            var query = new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.Company.Id == companyId).Select(x => x.Id);

            return this.Repository.FindAll<int>(query).ToList();
        }

        /// <summary>
        /// The get all by company id.
        /// </summary>
        /// <param name="firmId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Category}"/>.
        /// </returns>
        public IEnumerable<Category> GetAllByFirmId(int firmId)
        {
            QueryOver<Category, Category> query =
                new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.Company.Id == firmId);
            return this.Repository.FindAll(query).ToList();
        }

       

        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="searchPattern">
        /// The name.
        /// </param>
        /// <param name="firmId">
        /// The company Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalCount">
        /// The total count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
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
                queryOver = queryOver.Where(x => x.Company.Id == companyId);
            }

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this.fullTextModel.Search(searchPattern, typeof(Category), int.MaxValue).ToList();
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

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Category}"/>.
        /// </returns>
        public override IEnumerable<Category> GetAllByIds(List<int> ids)
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Id)
                    .IsIn(ids)
                    .Fetch(x => x.Company)
                    .Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Category}"/>.
        /// </returns>
        public override IFutureValue<Category> GetOneById(int id)
        {
            QueryOver<Category, Category> defaultQuery =
                new DefaultQueryOver<Category, int>().GetQueryOver().Where(x => x.Id == id).Fetch(x => x.Company).Eager;
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Category}"/>.
        /// </returns>
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