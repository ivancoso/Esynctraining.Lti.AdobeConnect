namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.FullText;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;
    using PDFAnnotation.Core.Business.Queries;
    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;

    public class ContactModel : BaseModel<Contact, int>
    {
        private readonly FullTextModel _fullTextModel;


        public ContactModel(FullTextModel fullTextModel, IRepository<Contact, int> repository)
            : base(repository)
        {
            _fullTextModel = fullTextModel ?? throw new ArgumentNullException(nameof(fullTextModel));
        }


        public override IEnumerable<Contact> GetAll()
        {
            var defaultQuery = new QueryOverContact().GetQueryOver();
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<Contact> Search(string pattern, int maxRows)
        {
            QueryOver<Contact, Contact> defaultQuery = new DefaultQueryOver<Contact, int>().GetQueryOver();

            if (maxRows == 0)
            {
                maxRows = int.MaxValue;
            }

            var searchIds = this._fullTextModel.Search(pattern, typeof(Contact), maxRows).ToList();
            defaultQuery = defaultQuery.WhereRestrictionOn(x => x.Id).IsIn(searchIds);
            return searchIds.Any() ? this.Repository.FindAll(defaultQuery).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<Contact> GetAllByCompanyAndNamePaged(string searchPattern, int companyId, int pageIndex, int pageSize, out int totalCount)
        {
            var searchIds = new List<int>();
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<Contact, int>().GetQueryOver();
            

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this._fullTextModel.Search(searchPattern, typeof(Contact), int.MaxValue).ToList();
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(searchIds);
            }

            if (companyId != 0)
            {
                CompanyContact companyContact = null;
                var queryOver2 = queryOver.JoinQueryOver(x => x.CompanyContacts, () => companyContact).Where(() => companyContact.Company.Id == companyId);
                var rowCountQuery = queryOver2.ToRowCountQuery();
                totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;

                if (pageSize > 0)
                {
                    var pagedQueryOver = queryOver2.Take(pageSize).Skip((pageIndex - 1) * pageSize);
                    return searchIds.Any() ? this.Repository.FindAll(pagedQueryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(pagedQueryOver);
                }

                return searchIds.Any() ? this.Repository.FindAll(queryOver2).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(queryOver2);
            }
            else
            {
                var rowCountQuery = queryOver.ToRowCountQuery();
                totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;

                if (pageSize > 0)
                {
                    var pagedQueryOver = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
                    return searchIds.Any() ? this.Repository.FindAll(pagedQueryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(pagedQueryOver);
                }

                return searchIds.Any() ? this.Repository.FindAll(queryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(queryOver);    
            }
        }

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

        public override void RegisterDelete(Contact entity, bool flush)
        {
            entity.Status = ContactStatusEnum.Deleted;
            base.RegisterSave(entity, flush);
        }

        public override void RegisterSave(Contact entity, bool flush, bool updateDateModified = true)
        {
            entity.DateModified = DateTime.Now;
            base.RegisterSave(entity, flush, updateDateModified);
        }

        public virtual IFutureValue<Contact> GetOneByEmail(string email)
        {
            var emailToLower = email.ToLower();
            var queryOver = new QueryOverContact().GetQueryOver().WhereRestrictionOn(x => x.Email).IsInsensitiveLike(emailToLower);
            return this.Repository.FindOne(queryOver);
        }

        public virtual IEnumerable<Contact> GetAllByEmails(List<string> emails)
        {
            List<Contact> result = new List<Contact>();
            var chunkedEmailsList = emails.Chunk(210);
            foreach (var chunk in chunkedEmailsList)
            {
                result.AddRange(GetByEmails(chunk));   
            }

            return result;
        }

        private IEnumerable<Contact> GetByEmails(IEnumerable<string> emails)
        {
            var queryOver = new QueryOverContact().GetQueryOver();
            var disjunction = new Disjunction();
            foreach (var email in emails)
            {
                disjunction.Add(Restrictions.On<Contact>(x => x.Email).IsInsensitiveLike(email));
            }

            queryOver.Where(disjunction);

            return this.Repository.FindAll(queryOver);
        }

        public virtual IEnumerable<Contact> GetAllByCompanyId(int companyId)
        {
            CompanyContact companyContact = null;
            var queryOver = new QueryOverContact().GetQueryOver().JoinQueryOver(x => x.CompanyContacts, () => companyContact).Where(() => companyContact.Company.Id == companyId);
            return this.Repository.FindAll(queryOver);
        }

        public virtual IEnumerable<Contact> GetAllByOrganizationId(Guid organizationId)
        {
            CompanyContact companyContact = null;
            var queryOver = new QueryOverContact().GetQueryOver().JoinQueryOver(x => x.CompanyContacts, () => companyContact).Where(() => companyContact.Company.OrganizationId == organizationId);
            return this.Repository.FindAll(queryOver);
        }

        public virtual IFutureValue<Contact> GetOneByPrincipalId(string principalId)
        {
            var principalIdToLower = principalId.ToLower();
            var queryOver = new QueryOverContact().GetQueryOver().WhereRestrictionOn(x => x.ACPrincipalId).IsInsensitiveLike(principalIdToLower);
            return this.Repository.FindOne(queryOver);
        }

        public override IFutureValue<Contact> GetOneById(int id)
        {
            var queryOver = new QueryOverContact().GetQueryOver().Where(x => x.Id == id);
            return this.Repository.FindOne(queryOver);
        }

        public IEnumerable<Contact> GetAllExceptDeleted()
        {
            var deleted = ContactStatusEnum.Deleted;
            var query = new DefaultQueryOver<Contact, int>().GetQueryOver().Where(x => x.Status != deleted);
            return this.Repository.FindAll(query);
        }

        public IEnumerable<CategoryContactDTO> GetAllByCategoriesIds(List<int> categoriesIds)
        {
            Contact contact = null;
            Category category = null;
            CategoryContactDTO dto = null;
            var query =
                new DefaultQueryOver<Contact, int>().GetQueryOver(() => contact)
                    .JoinQueryOver(() => contact.Categories, () => category)
                    .WhereRestrictionOn(() => category.Id)
                    .IsIn(categoriesIds)
                    .SelectList(
                        l =>
                        l.Select(() => contact.Id)
                            .WithAlias(() => dto.contactId)
                            .Select(() => category.Id)
                            .WithAlias(() => dto.categoryId))
                    .TransformUsing(Transformers.AliasToBean<CategoryContactDTO>());
            return this.Repository.FindAll<CategoryContactDTO>(query);
        }

    }

}