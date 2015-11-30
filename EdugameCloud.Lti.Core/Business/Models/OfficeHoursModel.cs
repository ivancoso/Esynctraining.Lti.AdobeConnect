﻿namespace EdugameCloud.Lti.Core.Business.Models
{
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.NHibernate;
    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The office hours model.
    /// </summary>
    public sealed class OfficeHoursModel : BaseModel<OfficeHours, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeHoursModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public OfficeHoursModel(IRepository<OfficeHours, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get by lms course meeting id.
        /// </summary>
        /// <param name="lmsUserId">
        /// The lms User Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue"/>.
        /// </returns>
        public IFutureValue<OfficeHours> GetByLmsUserId(int lmsUserId)
        {
            QueryOver<OfficeHours, OfficeHours> queryOver = QueryOver.Of<OfficeHours>().Where(s => s.LmsUser.Id == lmsUserId);

            return this.Repository.FindOne(queryOver);
        }

        #endregion

    }

}
