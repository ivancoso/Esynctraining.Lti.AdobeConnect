namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The AppletResult model.
    /// </summary>
    public class AppletResultModel : BaseModel<AppletResult, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletResultModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public AppletResultModel(IRepository<AppletResult, int> repository)
            : base(repository)
        {
        }

        #endregion


        /// <summary>
        /// The get one by ac session id and email.
        /// </summary>
        /// <param name="adobeConnectSessionId">
        /// The adobe connect session id.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{AppletResult}"/>.
        /// </returns>
        public IFutureValue<AppletResult> GetOneByACSessionIdAndEmail(int adobeConnectSessionId, string email)
        {
            var query =
                new DefaultQueryOver<AppletResult, int>().GetQueryOver()
                    .Where(x => x.ACSessionId == adobeConnectSessionId)
                    .AndRestrictionOn(x => x.Email)
                    .IsNotNull.AndRestrictionOn(x => x.Email)
                    .IsInsensitiveLike(email).Take(1);
            return this.Repository.FindOne(query);
        }
    }
}