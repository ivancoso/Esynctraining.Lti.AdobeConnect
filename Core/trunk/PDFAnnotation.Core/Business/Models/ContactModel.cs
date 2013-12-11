namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;

    using PDFAnnotation.Core.Business.Queries;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The user model.
    /// </summary>
    public class ContactModel : BaseModel<Contact, int>
    {
        /// <summary>
        /// The full text model.
        /// </summary>
        private readonly FullTextModel fullTextModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ContactModel(FullTextModel fullTextModel, IRepository<Contact, int> repository)
            : base(repository)
        {
            this.fullTextModel = fullTextModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{User}"/>.
        /// </returns>
        public override IEnumerable<Contact> GetAll()
        {
            var defaultQuery = new QueryOverContact().GetQueryOver();
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="pattern">
        /// The name.
        /// </param>
        /// <param name="maxRows">
        /// The max Rows.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Contact}"/>.
        /// </returns>
        public IEnumerable<Contact> Search(string pattern, int maxRows)
        {
            QueryOver<Contact, Contact> defaultQuery = new DefaultQueryOver<Contact, int>().GetQueryOver();

            if (maxRows == 0)
            {
                maxRows = int.MaxValue;
            }

            var searchIds = this.fullTextModel.Search(pattern, typeof(Contact), maxRows).ToList();
            defaultQuery = defaultQuery.WhereRestrictionOn(x => x.Id).IsIn(searchIds);
            return searchIds.Any() ? this.Repository.FindAll(defaultQuery).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all paged.
        /// </summary>
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
        public IEnumerable<Contact> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<Contact, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.OrderBy(x => x.FirstName).Asc.ThenBy(x => x.LastName).Asc.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        /// <param name="acp">
        /// The acp.
        /// </param>
        public void RegisterDelete(Contact entity, bool flush)
        {
            entity.Status = ContactStatusEnum.Deleted;
            base.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public override void RegisterSave(Contact entity, bool flush)
        {
            entity.DateModified = DateTime.Now;
            base.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The get one by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{User}"/>.
        /// </returns>
        public virtual IFutureValue<Contact> GetOneByEmail(string email)
        {
            var emailToLower = email.ToLower();
            var queryOver = new QueryOverContact().GetQueryOver().WhereRestrictionOn(x => x.Email).IsInsensitiveLike(emailToLower);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one by principalId.
        /// </summary>
        /// <param name="principalId">
        /// The principalId.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{User}"/>.
        /// </returns>
        public virtual IFutureValue<Contact> GetOneByPrincipalId(string principalId)
        {
            var principalIdToLower = principalId.ToLower();
            var queryOver = new QueryOverContact().GetQueryOver().WhereRestrictionOn(x => x.ACPrincipalId).IsInsensitiveLike(principalIdToLower);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Contact}"/>.
        /// </returns>
        public override IFutureValue<Contact> GetOneById(int id)
        {
            var queryOver = new QueryOverContact().GetQueryOver().Where(x => x.Id == id);
            return this.Repository.FindOne(queryOver);
        }

        #endregion

        /// <summary>
        /// The get all by role.
        /// </summary>
        /// <param name="contactTypeId">
        /// The contact type id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Contact> GetAllByRole(int contactTypeId)
        {
            var query =
                new QueryOverContact().GetQueryOver()
                                      .Where(x => x.ContactType.Id == contactTypeId);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all by roles.
        /// </summary>
        /// <param name="contactTypeIds">
        /// The contact Type Ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Contact> GetAllByRoles(List<int> contactTypeIds)
        {
            var query =
                new QueryOverContact().GetQueryOver()
                                      .WhereRestrictionOn(x => x.ContactType.Id).IsIn(contactTypeIds);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all including billing.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Contact> GetAllExceptDeleted()
        {
            var deleted = ContactStatusEnum.Deleted;
            var query = new DefaultQueryOver<Contact, int>().GetQueryOver().Where(x => x.Status != deleted);
            return this.Repository.FindAll(query);
        }

    }
}