namespace EdugameCloud.Lti.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;

    /// <summary>
    /// The LMS course meeting model.
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
        /// The company LMS id</param>
        /// <param name="courseId">
        /// The course id
        /// </param>
        /// <param name="scoId">
        /// The sco id.
        /// </param>
        /// <returns>
        /// The canvas AC meeting
        /// </returns>
        public IFutureValue<LmsCourseMeeting> GetOneByCourseAndScoId(int companyLmsId, int courseId, string scoId)
        {
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => x.CompanyLms != null && x.CompanyLms.Id == companyLmsId && x.CourseId == courseId &&
                    (((x.ScoId != null) && (x.ScoId == scoId)) || 
                     (x.OfficeHours != null && oh.ScoId == scoId)))
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get one by user and type.
        /// </summary>
        /// <param name="companyLmsId">
        /// The company lms id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue"/>.
        /// </returns>
        public IFutureValue<LmsCourseMeeting> GetOneByUserAndType(int companyLmsId, string userId, int type)
        {
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.InnerJoin)
                .JoinAlias(() => oh.LmsUser, () => u, JoinType.InnerJoin)
                .Where(() => x.CompanyLms != null && x.CompanyLms.Id == companyLmsId && x.LmsMeetingType == type &&
                     (x.OfficeHours != null && u.UserId == userId))
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get one by course and type.
        /// </summary>
        /// <param name="companyLmsId">
        /// The company lms id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue"/>.
        /// </returns>
        public IFutureValue<LmsCourseMeeting> GetOneByCourseAndType(int companyLmsId, int courseId, int type)
        {
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.CompanyLms.Id == companyLmsId && x.CourseId == courseId && (x.LmsMeetingType == type)).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get all by course id.
        /// </summary>
        /// <param name="companyLmsId">
        /// The company lms id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<LmsCourseMeeting> GetAllByCourseId(int companyLmsId, int courseId)
        {
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.CompanyLms.Id == companyLmsId && x.CourseId == courseId);
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get one by meeting id.
        /// </summary>
        /// <param name="companyLmsId">
        /// The company LMS id.
        /// </param>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsCourseMeeting}"/>.
        /// </returns>
        public IFutureValue<LmsCourseMeeting> GetOneByMeetingId(int companyLmsId, string meetingId)
        {
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.CompanyLms != null && x.CompanyLms.Id == companyLmsId && x.ScoId == meetingId).Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        /// <summary>
        /// The get all by meeting id.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{LmsCourseMeeting}"/>.
        /// </returns>
        public IEnumerable<LmsCourseMeeting> GetAllByMeetingId(string meetingId)
        {
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => (((x.ScoId != null) && (x.ScoId == meetingId)) ||
                     (x.OfficeHours != null && oh.ScoId == meetingId)));
            return this.Repository.FindAll(defaultQuery);

            /*
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.ScoId == meetingId);
            return this.Repository.FindAll(defaultQuery);
            */
        }
    }
}
