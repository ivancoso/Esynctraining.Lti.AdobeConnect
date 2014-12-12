namespace EdugameCloud.Lti.Business.Models
{
    using System;
    using System.Collections.Generic;

    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    ///     The LMS User Session model.
    /// </summary>
    public class LmsUserSessionModel : BaseModel<LmsUserSession, Guid>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserSessionModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsUserSessionModel(IRepository<LmsUserSession, Guid> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by company and user.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUserSession}"/>.
        /// </returns>
        public IFutureValue<LmsUserSession> GetOneByCompanyAndUserAndCourse(int companyId, int userId, int courseId)
        {
            var queryOver =
                new DefaultQueryOver<LmsUserSession, Guid>().GetQueryOver()
                    .Where(c => c.LmsUser != null && c.LmsUser.Id == userId && c.CompanyLms.Id == companyId && c.LmsCourseId == courseId);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get all older then.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{LmsUserSession}"/>.
        /// </returns>
        public IEnumerable<LmsUserSession> GetAllOlderThen(DateTime dateTime)
        {
            var queryOver =
                new DefaultQueryOver<LmsUserSession, Guid>().GetQueryOver()
                    .Where(
                        Restrictions.Or(
                            Restrictions.And(
                                Restrictions.IsNotNull(Projections.Property<LmsUserSession>(x => x.DateModified)),
                                Restrictions.Le(Projections.Property<LmsUserSession>(x => x.DateModified), dateTime)),
                            Restrictions.Le(Projections.Property<LmsUserSession>(x => x.DateCreated), dateTime)));
            return this.Repository.FindAll(queryOver);
        }
    }
}