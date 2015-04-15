namespace EdugameCloud.Lti.Core.Business.Models
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The LMS provider model.
    /// </summary>
    public sealed class LmsProviderModel : BaseModel<LmsProvider, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsProviderModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsProviderModel(IRepository<LmsProvider, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all active.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="LmsProvider"/>.
        /// </returns>
        public IFutureValue<LmsProvider> GetOneByName(string name)
        {
            var query = new DefaultQueryOver<LmsProvider, int>()
                .GetQueryOver()
                .WhereRestrictionOn(x => x.ShortName)
                .IsInsensitiveLike(name, MatchMode.Exact);
            return this.Repository.FindOne(query);
        }
    }

}
