using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.Extensions;
    using NHibernate;
    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class CompanyModel : BaseModel<Company, int>
    {
        public CompanyModel(IRepository<Company, int> repository)
            : base(repository)
        {
        }


        public IEnumerable<Company> Search(string name)
        {
            QueryOver<Company, Company> defaultQuery =
                new DefaultQueryOver<Company, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.CompanyName)
                    .IsInsensitiveLike("%" + name + "%");
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<int> GetAllIds()
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().Select(x => x.Id);
            return this.Repository.FindAll<int>(query).ToList().Distinct();
        }

        public IFutureValue<Company> GetOneByName(string companyName)
        {
            var companyNameToLower = companyName.Return(x => x.ToLowerInvariant(), string.Empty);
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().WhereRestrictionOn(x => x.CompanyName).IsInsensitiveLike(companyNameToLower);
            return this.Repository.FindOne(query);
        }

        public IFutureValue<Company> GetOneByOrganizationId(Guid organizationId)
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().Where(x => x.OrganizationId == organizationId).Take(1);
            return this.Repository.FindOne(query);
        }

        public IEnumerable<Company> GetAllForUser(int userId)
        {
            CompanyContact companyContact = null;
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().JoinQueryOver(x => x.CompanyContacts, () => companyContact).Where(() => companyContact.Contact.Id == userId);
            return this.Repository.FindAll(query);
        }

        public IEnumerable<Company> GetAllByOrganizationIds(List<Guid> organizationIds)
        {
            var query = new DefaultQueryOver<Company, int>().GetQueryOver().WhereRestrictionOn(x => x.OrganizationId).IsIn(organizationIds);
            return this.Repository.FindAll(query);
        }

    }

}