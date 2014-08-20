namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    /// 
    /// </summary>
    public class CanvasACMeetingModel :BaseModel<CanvasACMeeting, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasACMeetingModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CanvasACMeetingModel(IRepository<CanvasACMeeting, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// Gets one by context id
        /// </summary>
        /// <param name="contextId">
        /// The context id
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<CanvasACMeeting> GetOneByContextId(string contextId)
        {
            var defaultQuery = new DefaultQueryOver<CanvasACMeeting, int>().GetQueryOver()
                .Where(x => x.ContextId == contextId).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }
    }
}
