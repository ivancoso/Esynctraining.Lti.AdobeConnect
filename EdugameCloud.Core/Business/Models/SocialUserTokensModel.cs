using NHibernate.Criterion;

namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The SubModuleCategory model.
    /// </summary>
    public class SocialUserTokensModel : BaseModel<SocialUserTokens, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialUserTokensModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SocialUserTokensModel(IRepository<SocialUserTokens, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{SocialUserTokens}"/>.
        /// </returns>
        public IFutureValue<SocialUserTokens> GetOneByKey(string key)
        {
            var query =
                new DefaultQueryOver<SocialUserTokens, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Key)
                    .IsInsensitiveLike(key, MatchMode.Exact).Take(1);
            return this.Repository.FindOne(query);
        }
    }
}