namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    /// The LMS user model
    /// </summary>
    public class LmsUserModel : BaseModel<LmsUser, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsUserModel(IRepository<LmsUser, int> repository) : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get one by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="companyLmsId">
        /// The company Lms Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue"/>.
        /// </returns>
        public IFutureValue<LmsUser> GetOneByUserIdAndCompanyLms(int userId, int companyLmsId)
        {
            var queryOver = new DefaultQueryOver<LmsUser, int>().GetQueryOver().Where(u => u.UserId == userId && u.CompanyLms.Id == companyLmsId);
            return this.Repository.FindOne(queryOver);
        }

        #endregion
    }
}
