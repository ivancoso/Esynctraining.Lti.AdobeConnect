namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    /// The lms course meeting model.
    /// </summary>
    public class LmsCourseMeetingModel : BaseModel<LmsCourseMeeting, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsCourseMeetingModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsCourseMeetingModel(IRepository<LmsCourseMeeting, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// Gets one by course id
        /// </summary>
        /// <param name="companyLmsId">
        /// The company lms id</param>
        /// <param name="courseId">
        /// The course id
        /// </param>
        /// <returns>
        /// The canvas ac meeting
        /// </returns>
        public IFutureValue<LmsCourseMeeting> GetOneByCourseId(int companyLmsId, int courseId)
        {
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.CompanyLmsId == companyLmsId && x.CourseId == courseId).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }
    }
}
