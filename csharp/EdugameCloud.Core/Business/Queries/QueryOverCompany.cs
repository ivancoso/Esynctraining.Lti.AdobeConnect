namespace EdugameCloud.Core.Business.Queries
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate.Queries;
    using NHibernate.Criterion;

    public class QueryOverCompany : DefaultQueryOver<Company, int>
    {
        protected override QueryOver<Company, Company> Apply(QueryOver<Company, Company> queryOver)
        {
            var deletedStatus = CompanyStatus.Deleted;
            return base.Apply(queryOver).Where(x => x.Status != deletedStatus);
        }

    }

}