namespace EdugameCloud.Core.Business.Queries
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    public class QueryOverCompany : DefaultQueryOver<Company, int>
    {
        #region Methods

        /// <summary>
        ///     The apply.
        /// </summary>
        /// <param name="queryOver">
        ///     The query over.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryOver" />.
        /// </returns>
        protected override QueryOver<Company, Company> Apply(QueryOver<Company, Company> queryOver)
        {
            var deletedStatus = CompanyStatus.Deleted;
            return base.Apply(queryOver).Where(x => x.Status != deletedStatus);
        }

        #endregion
    }
}