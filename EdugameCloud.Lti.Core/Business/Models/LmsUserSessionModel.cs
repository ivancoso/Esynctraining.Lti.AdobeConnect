namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    /// <summary>
    ///     The LMS User Session model.
    /// </summary>
    public sealed class LmsUserSessionModel : BaseModel<LmsUserSession, Guid>
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
        
        public IFutureValue<LmsUserSession> GetOneByCompanyAndUserAndCourse(int userId, int courseId)
        {
            var queryOver =
                new DefaultQueryOver<LmsUserSession, Guid>().GetQueryOver()
                    .Where(c => c.LmsUser.Id == userId && c.LmsCourseId == courseId);
            return this.Repository.FindOne(queryOver);
        }

        public IFutureValue<LmsUserSession> GetByIdWithRelated(Guid sessionId)
        {
            var queryOver =
                new DefaultQueryOver<LmsUserSession, Guid>().GetQueryOver()
                .Fetch(x => x.LmsCompany).Eager
                .Fetch(x => x.LmsCompany.Settings).Eager
                .Fetch(x => x.LmsCompany.AdminUser).Eager
                .Where(c => c.Id == sessionId)
                .TransformUsing(Transformers.DistinctRootEntity);

            return this.Repository.FindOne(queryOver);
        }

        public LmsUserSession GetReadOnlyByIdWithRelated(Guid sessionId)
        {
            using (var txn = this.Repository.Session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var queryOver =
                    new DefaultQueryOver<LmsUserSession, Guid>().GetQueryOver()
                    .Fetch(x => x.LmsCompany).Eager
                    .Fetch(x => x.LmsCompany.AdminUser).Eager
                    .Where(c => c.Id == sessionId)
                    .TransformUsing(Transformers.DistinctRootEntity);

                //return this.Repository.FindOne(queryOver).Value;
                LmsUserSession result = this.Repository.FindOne(queryOver).Value;// queryOver.GetExecutableQueryOver(session).FutureValue<LmsUserSession>().Value;
                txn.Rollback();
                return result;
            }
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