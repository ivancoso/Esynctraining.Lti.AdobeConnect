namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;
    using NHibernate.Transform;

    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContact model class.
    /// </summary>
    public class CompanyContactModel : BaseModel<CompanyContact, int>
    {
        /// <summary>
        /// The full text model.
        /// </summary>
        private readonly FullTextModel fullTextModel;

        /// <summary>
        /// The case repository.
        /// </summary>
        private readonly IRepository<Category, int> caseRepository;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactModel"/> class.
        /// </summary>
        /// <param name="fullTextModel">
        /// The full Text Model.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="caseRepository">
        /// The case Repository.
        /// </param>
        public CompanyContactModel(FullTextModel fullTextModel, IRepository<CompanyContact, int> repository, IRepository<Category, int> caseRepository)
            : base(repository)
        {
            this.fullTextModel = fullTextModel;
            this.caseRepository = caseRepository;
        }

        #endregion

        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="searchPattern">
        /// The search Pattern.
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
        public IEnumerable<CompanyContact> GetAllByFirmPaged(string searchPattern, int firmId, int pageIndex, int pageSize, out int totalCount)
        {
            var searchIds = new List<int>();

            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<CompanyContact, int>().GetQueryOver();
            if (firmId != 0)
            {
                queryOver = queryOver.Where(x => x.Company.Id == firmId);
            }

            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            Contact contact = null;
            var pagedQuery = queryOver.JoinQueryOver(x => x.Contact, () => contact);

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this.fullTextModel.Search(searchPattern, typeof(Contact), int.MaxValue).ToList();
                pagedQuery = pagedQuery.WhereRestrictionOn(() => contact.Id).IsIn(searchIds);
            }

            var resultedPagedQuery = pagedQuery.OrderByAlias(() => contact.FirstName)
                    .Asc.ThenByAlias(() => contact.LastName)
                    .Asc.Fetch(x => x.Contact).Eager;

            if (pageSize > 0)
            {
                var res = this.Repository.FindAll(resultedPagedQuery.Take(pageSize).Skip((pageIndex - 1) * pageSize)).ToList();
                return searchIds.Any() ? res.OrderBy(x => searchIds.IndexOf(x.Contact.Id)).ToList() : res;
            }

            return searchIds.Any() ? this.Repository.FindAll(resultedPagedQuery).ToList().OrderBy(x => searchIds.IndexOf(x.Contact.Id)) : this.Repository.FindAll(resultedPagedQuery);
        }

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="firmId">
        /// The company Id.
        /// </param>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CompanyContact}"/>.
        /// </returns>
        public IEnumerable<CompanyContact> GetAllByFirmAndIds(int firmId, List<int> ids)
        {
            var query = new DefaultQueryOver<CompanyContact, int>().GetQueryOver().Where(x => x.Company.Id == firmId).JoinQueryOver(x => x.Contact).WhereRestrictionOn(x => x.Id).IsIn(ids).Fetch(x => x.Contact).Eager;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all by contact ids.
        /// </summary>
        /// <param name="contactIds">
        /// The contact ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CompanyContact}"/>.
        /// </returns>
        public IEnumerable<CompanyContact> GetAllByContactIds(List<int> contactIds)
        {
            var query = new DefaultQueryOver<CompanyContact, int>().GetQueryOver().WhereRestrictionOn(x => x.Contact.Id).IsIn(contactIds);
            return this.Repository.FindAll(query);
        }
    }
}