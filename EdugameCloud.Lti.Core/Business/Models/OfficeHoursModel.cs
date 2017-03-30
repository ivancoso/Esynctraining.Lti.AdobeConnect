namespace EdugameCloud.Lti.Core.Business.Models
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

        public IFutureValue<OfficeHours> GetByLmsUserId(int lmsUserId)
        {
            QueryOver<OfficeHours, OfficeHours> queryOver = QueryOver.Of<OfficeHours>().Where(s => s.LmsUser.Id == lmsUserId);

            return this.Repository.FindOne(queryOver);
        }

        #endregion

    }

}
