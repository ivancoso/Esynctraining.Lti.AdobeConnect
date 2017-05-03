namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Linq;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;

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
        
        public LmsCourseMeeting GetOneByCourseAndId(int companyLmsId, int courseId, long id)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException(nameof(companyLmsId));
            if (courseId == 0)
                throw new ArgumentOutOfRangeException(nameof(courseId));
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            LmsCourseMeeting x = null;
            //OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                //.JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => x.Id == id && x.LmsCompanyId == companyLmsId && x.CourseId == courseId)
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value;
        }

        public bool ContainsByCompanyAndScoId(ILmsLicense lmsCompany, string scoId, int excludedLmsCourseMeetingId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", nameof(scoId));

            var defaultQuery = GetByCompanyAndScoIdQuery(lmsCompany, scoId, excludedLmsCourseMeetingId)
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value != null;
        }

        public int CourseCountByCompanyAndScoId(ILmsLicense lmsCompany, string scoId, int excludedLmsCourseMeetingId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", nameof(scoId));

            var defaultQuery = GetByCompanyAndScoIdQuery(lmsCompany, scoId, excludedLmsCourseMeetingId);

            var rowCountQuery = defaultQuery
                .Select(Projections.Distinct(Projections.Property<LmsCourseMeeting>(s => s.CourseId)))
                .TransformUsing(Transformers.DistinctRootEntity)
                .ToRowCountQuery();
            return this.Repository.FindOne<int>(rowCountQuery).Value;
        }

        public IEnumerable<LmsCourseMeeting> GetByCompanyAndScoId(ILmsLicense lmsCompany, string scoId, int excludedLmsCourseMeetingId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", nameof(scoId));
            
            var defaultQuery = GetByCompanyAndScoIdQuery(lmsCompany, scoId, excludedLmsCourseMeetingId);
            return this.Repository.FindAll(defaultQuery);
        }

        private QueryOver<LmsCourseMeeting, LmsCourseMeeting> GetByCompanyAndScoIdQuery(ILmsLicense lmsCompany, string scoId, int excludedLmsCourseMeetingId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", nameof(scoId));

            // NOTE: check only licenses of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenseAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenseAc.Host).Select(c => c.Id).ToArray();

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            QueryOver<LmsCourseMeeting, LmsCourseMeeting> defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .WhereRestrictionOn(() => x.LmsCompanyId).IsIn(companyLicenses)
                .And(() => x.Id != excludedLmsCourseMeetingId)
                .And(() =>
                    ((x.ScoId != null) && (x.ScoId == scoId)) ||
                     (x.OfficeHours != null && oh.ScoId == scoId))
                .TransformUsing(Transformers.DistinctRootEntity);

            return defaultQuery;
        }

        public IEnumerable<LmsCourseMeeting> GetByCompanyWithAudioProfiles(ILmsLicense lmsCompany)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            // NOTE: check only licenses of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenseAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenseAc.Host).Select(c => c.Id).ToArray();

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .WhereRestrictionOn(() => x.LmsCompanyId).IsIn(companyLicenses)
                .And(() => x.AudioProfileId != null)
                .TransformUsing(Transformers.DistinctRootEntity);

            return this.Repository.FindAll(defaultQuery);
        }


        public LmsCourseMeeting GetLtiCreatedByCompanyAndScoId(ILmsLicense lmsCompany, string scoId)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("scoId can not be empty", nameof(scoId));

            // NOTE: check only licenses of the company with the same AC!!
            var query = from c in this.Repository.Session.Query<LmsCompany>()
                        where c.CompanyId == lmsCompany.CompanyId
                        select new { c.Id, c.AcServer };
            var currentLicenseAc = new Uri(lmsCompany.AcServer);
            var companyLicenses = query.ToArray().Where(c => new Uri(c.AcServer).Host == currentLicenseAc.Host).Select(c => c.Id).ToArray();

            // NOTE: return only not-reused meeting - created from LTI
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                //.Clone()
                .WhereRestrictionOn(() => x.LmsCompanyId).IsIn(companyLicenses)
                .Where(() =>
                    (x.Reused == null || x.Reused == false)
                    &&
                    (((x.ScoId != null) && (x.ScoId == scoId)) ||
                     (x.OfficeHours != null && oh.ScoId == scoId)))
                .Take(1);

            return this.Repository.FindOne(defaultQuery).Value;
        }

        public IFutureValue<LmsCourseMeeting> GetOneByUserAndType(int companyLmsId, string userId, LmsMeetingType type)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException(nameof(companyLmsId));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("userId can not be empty", nameof(userId));
            if (type <= 0)
                throw new ArgumentOutOfRangeException(nameof(type));

            int typeValue = (int)type;
            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.InnerJoin)
                .JoinAlias(() => oh.LmsUser, () => u, JoinType.InnerJoin)
                .Where(() => x.LmsCompanyId == companyLmsId 
                    && x.LmsMeetingType == typeValue 
                    && (x.OfficeHours != null && u.UserId == userId))
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }

        //public IFutureValue<LmsCourseMeeting> GetOneByCourseAndType(int companyLmsId, int courseId, LmsMeetingType type)
        //{
        //    if (companyLmsId <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(companyLmsId));
        //    if (courseId == 0)
        //        throw new ArgumentOutOfRangeException(nameof(courseId));
        //    if (type <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(type));

        //    int typeValue = (int)type;
        //    var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
        //        .Where(x => x.LmsCompanyId == companyLmsId && x.CourseId == courseId && x.LmsMeetingType == typeValue).Take(1);
        //    return this.Repository.FindOne(defaultQuery);
        //}

        public IEnumerable<LmsCourseMeeting> GetAllByCourseId(int companyLmsId, int courseId)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException(nameof(companyLmsId));
            if (courseId == 0)
                throw new ArgumentOutOfRangeException(nameof(courseId));

            int seminars = (int)LmsMeetingType.Seminar;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.LmsCompanyId == companyLmsId && x.CourseId == courseId && x.LmsMeetingType != seminars);
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<LmsCourseMeeting> GetSeminarsByCourseId(int companyLmsId, int courseId)
        {
            if (companyLmsId <= 0)
                throw new ArgumentOutOfRangeException(nameof(companyLmsId));
            if (courseId == 0)
                throw new ArgumentOutOfRangeException(nameof(courseId));

            int seminars = (int)LmsMeetingType.Seminar;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>().GetQueryOver()
                .Where(x => x.LmsCompanyId == companyLmsId && x.CourseId == courseId && x.LmsMeetingType == seminars);
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
                throw new ArgumentException("meetingId can not be empty", nameof(meetingScoId));

            LmsCourseMeeting x = null;
            OfficeHours oh = null;
            var defaultQuery = new DefaultQueryOver<LmsCourseMeeting, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.OfficeHours, () => oh, JoinType.LeftOuterJoin)
                .Where(() => (((x.ScoId != null) && (x.ScoId == meetingScoId)) ||
                     (x.OfficeHours != null && oh.ScoId == meetingScoId)));
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<LmsCourseMeeting> GetAllByOfficeHoursId(int officeHoursId)
        {
            if (officeHoursId <= 0)
                throw new ArgumentOutOfRangeException(nameof(officeHoursId));

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
