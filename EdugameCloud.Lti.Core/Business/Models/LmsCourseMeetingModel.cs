namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Linq;
    using NHibernate.SqlCommand;

    /// <summary>
    /// The LMS course meeting model.
    /// </summary>
    public sealed class LmsCourseMeetingModel : BaseModel<LmsCourseMeeting, int>
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
        
        public LmsCourseMeeting GetOneByCourseAndId(int companyLmsId, int courseId, int id)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException("companyLmsId");
            if (courseId == 0)
                throw new ArgumentOutOfRangeException("courseId");
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id");

            LmsCourseMeeting x = null;
            //OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                //.JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => x.Id == id && x.LmsCompany.Id == companyLmsId && x.CourseId == courseId)
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value;
        }

        public bool ContainsByCompanyAndScoId(LmsCompany lmsCompany, string scoId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", "scoId");

            // NOTE: check only licences of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenceAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenceAc.Host).Select(c => c.Id).ToArray();

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsCompany lms = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                //.Clone()
                .JoinAlias(() => x.LmsCompany, () => lms, JoinType.InnerJoin)
                .WhereRestrictionOn(() => lms.Id).IsIn(companyLicenses)
                .And(() => 
                    ((x.ScoId != null) && (x.ScoId == scoId)) ||
                     (x.OfficeHours != null && oh.ScoId == scoId))
                  
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value != null;
        }

        public IEnumerable<LmsCourseMeeting> GetByCompanyAndScoId(LmsCompany lmsCompany, string scoId, int excludedLmsCourseMeetingId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", "scoId");

            // NOTE: check only licences of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenceAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenceAc.Host).Select(c => c.Id).ToArray();

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsCompany lms = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                //.Clone()
                .JoinAlias(() => x.LmsCompany, () => lms, JoinType.InnerJoin)
                .Where(() => x.Id != excludedLmsCourseMeetingId)
                .AndRestrictionOn(() => lms.Id).IsIn(companyLicenses)
                .And(() =>
                    ((x.ScoId != null) && (x.ScoId == scoId)) ||
                     (x.OfficeHours != null && oh.ScoId == scoId));

            return this.Repository.FindAll(defaultQuery);
        }


        public LmsCourseMeeting GetLtiCreatedByCompanyAndScoId(LmsCompany lmsCompany, string scoId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", "scoId");

            // NOTE: check only licences of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenceAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenceAc.Host).Select(c => c.Id).ToArray();

            // NOTE: return only not-reused meeting - created from LTI
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsCompany lms = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                //.Clone()
                .JoinAlias(() => x.LmsCompany, () => lms, JoinType.InnerJoin)
                .WhereRestrictionOn(() => lms.Id).IsIn(companyLicenses)
                .Where(() =>
                    (x.Reused == null || x.Reused == false)
                    &&
                    (((x.ScoId != null) && (x.ScoId == scoId)) ||
                     (x.OfficeHours != null && oh.ScoId == scoId)))
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value;
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
        public IFutureValue<LmsCourseMeeting> GetOneByUserAndType(int companyLmsId, string userId, LmsMeetingType type)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException("companyLmsId");
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("userId can not be empty", "userId");
            if (type <= 0)
                throw new ArgumentOutOfRangeException("type");

            int typeValue = (int)type;
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.InnerJoin)
                .JoinAlias(() => oh.LmsUser, () => u, JoinType.InnerJoin)
                .Where(() => x.LmsCompany.Id == companyLmsId 
                    && x.LmsMeetingType == typeValue 
                    && (x.OfficeHours != null && u.UserId == userId))
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
        public IFutureValue<LmsCourseMeeting> GetOneByCourseAndType(int companyLmsId, int courseId, LmsMeetingType type)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException("companyLmsId");
            if (courseId == 0)
                throw new ArgumentOutOfRangeException("courseId");
            if (type <= 0)
                throw new ArgumentOutOfRangeException("type");

            int typeValue = (int)type;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.LmsCompany.Id == companyLmsId && x.CourseId == courseId && x.LmsMeetingType == typeValue).Take(1);
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
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException("companyLmsId");
            if (courseId == 0)
                throw new ArgumentOutOfRangeException("courseId");

            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.LmsCompany.Id == companyLmsId && x.CourseId == courseId);
            return this.Repository.FindAll(defaultQuery);
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
        public IEnumerable<LmsCourseMeeting> GetAllByMeetingId(string meetingScoId)
        {
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("meetingId can not be empty", "meetingScoId");

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => (((x.ScoId != null) && (x.ScoId == meetingScoId)) ||
                     (x.OfficeHours != null && oh.ScoId == meetingScoId)));
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by office hours id.
        /// </summary>
        /// <param name="officeHoursId">
        /// The office hours id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<LmsCourseMeeting> GetAllByOfficeHoursId(int officeHoursId)
        {
            if (officeHoursId <= 0)
                throw new ArgumentOutOfRangeException("officeHoursId");

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => x.OfficeHours != null && oh.Id == officeHoursId);
            return this.Repository.FindAll(defaultQuery);
        }

    }

}
