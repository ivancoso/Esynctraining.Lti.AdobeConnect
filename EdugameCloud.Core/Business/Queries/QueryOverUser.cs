namespace EdugameCloud.Core.Business.Queries
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    public class QueryOverUser : DefaultQueryOver<User, int>
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
        protected override QueryOver<User, User> Apply(QueryOver<User, User> queryOver)
        {
            var deletedStatus = UserStatus.Deleted;
            return base.Apply(queryOver).Where(x => x.Status != deletedStatus);
        }

        #endregion
    }
}