namespace EdugameCloud.Core.Business.Queries
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate.Queries;
    using NHibernate.Criterion;

    public class QueryOverUser : DefaultQueryOver<User, int>
    {
        protected override QueryOver<User, User> Apply(QueryOver<User, User> queryOver)
        {
            var deletedStatus = UserStatus.Deleted;
            return base.Apply(queryOver).Where(x => x.Status != deletedStatus);
        }

    }

}