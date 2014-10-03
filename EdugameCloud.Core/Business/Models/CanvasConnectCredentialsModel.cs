namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    /// The canvas connect credentials model
    /// </summary>
    public class CanvasConnectCredentialsModel : BaseModel<CanvasConnectCredentials, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasConnectCredentialsModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CanvasConnectCredentialsModel(IRepository<CanvasConnectCredentials, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// Gets one by domain
        /// </summary>
        /// <param name="canvasDomain">
        /// The canvas domain
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<CanvasConnectCredentials> GetOneByDomain(string canvasDomain)
        {
            var defaultQuery = new DefaultQueryOver<CanvasConnectCredentials, int>().GetQueryOver()
                .Where(x => x.CanvasDomain == canvasDomain).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }
    }
}
