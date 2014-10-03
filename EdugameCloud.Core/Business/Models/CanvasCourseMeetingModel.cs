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
    public class CanvasCourseMeetingModel :BaseModel<CanvasCourseMeeting, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCourseMeetingModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CanvasCourseMeetingModel(IRepository<CanvasCourseMeeting, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// Gets one by course id
        /// </summary>
        /// <param name="credentialsId">The credentials id</param>
        /// <param name="courseId">
        /// The course id
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<CanvasCourseMeeting> GetOneByCourseId(int credentialsId, int courseId)
        {
            var defaultQuery = new DefaultQueryOver<CanvasCourseMeeting, int>().GetQueryOver()
                .Where(x => x.CanvasConnectCredentialsId == credentialsId && x.CourseId == courseId).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }
    }
}
