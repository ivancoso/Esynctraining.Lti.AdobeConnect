namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    /// <summary>
    ///     The ContactLoginHistory model.
    /// </summary>
    public class UserLoginHistoryModel : BaseModel<UserLoginHistory, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginHistoryModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public UserLoginHistoryModel(IRepository<UserLoginHistory, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{UserLoginHistory}"/>.
        /// </returns>
        public override IEnumerable<UserLoginHistory> GetAll()
        {
            var query = new DefaultQueryOver<UserLoginHistory, int>().GetQueryOver().Fetch(x => x.User).Eager;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all for user.
        /// </summary>
        /// <param name="userIds">
        /// The user Ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{UserLoginHistory}"/>.
        /// </returns>
        public IEnumerable<UserLoginHistory> GetAllForUsers(List<int> userIds)
        {
            var query =
                new DefaultQueryOver<UserLoginHistory, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.User.Id).IsIn(userIds)
                                                     .Fetch(x => x.User).Eager;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all for user.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{UserLoginHistory}"/>.
        /// </returns>
        public IEnumerable<UserLoginHistory> GetAllForUser(int userId)
        {
            var query =
                new DefaultQueryOver<UserLoginHistory, int>().GetQueryOver()
                                                     .Where(x => x.User.Id == userId)
                                                     .Fetch(x => x.User).Eager;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get all by contact paged.
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
        /// The <see cref="IEnumerable{Case}"/>.
        /// </returns>
        public IEnumerable<UserLoginHistory> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            User @usr = null;
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var resultedQueryOver = new DefaultQueryOver<UserLoginHistory, int>().GetQueryOver().OrderBy(x => x.DateCreated).Desc.Fetch(x => x.User).Eager;
            var rowCountQuery = resultedQueryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            QueryOver<UserLoginHistory> pagedQuery = resultedQueryOver;
            if (pageSize > 0)
            {
                pagedQuery = pagedQuery.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            }

            return this.Repository.FindAll(pagedQuery);
        }

    }
}