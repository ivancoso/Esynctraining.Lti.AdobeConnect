using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class CompanyContactModel : BaseModel<CompanyContact, int>
    {
        public CompanyContactModel(IRepository<CompanyContact, int> repository)
            : base(repository)
        {
        }


        public IEnumerable<CompanyContact> GetAllByCompanyId(int companyId)
        {
            QueryOver<CompanyContact, CompanyContact> query =
                new DefaultQueryOver<CompanyContact, int>().GetQueryOver()
                    .Where(x => x.Company.Id == companyId)
                    .Fetch(x => x.Contact)
                    .Eager;
            return this.Repository.FindAll(query);
        }

        public IEnumerable<CompanyContact> GetAllByCompanyIds(List<int> companyIds)
        {
            QueryOver<CompanyContact, CompanyContact> query =
                new DefaultQueryOver<CompanyContact, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Company.Id)
                    .IsIn(companyIds)
                    .Fetch(x => x.Contact)
                    .Eager;
            return this.Repository.FindAll(query);
        }

    }

}