namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.SqlCommand;

    /// <summary>
    /// The LMS user parameters model
    /// </summary>
    public class LmsUserParametersModel : BaseModel<LmsUserParameters, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsUserParametersModel(IRepository<LmsUserParameters, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// The get one by AC id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUserParameters}"/>.
        /// </returns>
        public IFutureValue<LmsUserParameters> GetOneByAcId(string id)
        {
            var queryOver = new DefaultQueryOver<LmsUserParameters, int>().GetQueryOver().Where(x => x.AcId == id);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one for login.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="adobeConectDomain">
        /// The ac domain.
        /// </param>
        /// <param name="courseId">
        /// The course Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUserParameters}"/>.
        /// </returns>
        public IFutureValue<LmsUserParameters> GetOneForLogin(string id, string adobeConectDomain, int courseId)
        {
            CompanyLms clms = null;
            var queryOver = new DefaultQueryOver<LmsUserParameters, int>().GetQueryOver()
                .Where(x => x.AcId == id && x.Course == courseId)
                .JoinQueryOver(x => x.CompanyLms, () => clms, JoinType.InnerJoin)
                .WhereRestrictionOn(x => clms.AcServer).IsInsensitiveLike(adobeConectDomain);
            return this.Repository.FindOne(queryOver);
        }

        #endregion

        /// <summary>
        /// The get one by AC id course id and LMS user id.
        /// </summary>
        /// <param name="adobeConnectUserId">
        /// The adobe connect user id.
        /// </param>
        /// <param name="lmsCourseId">
        /// The LMS course id.
        /// </param>
        /// <param name="lmsUserId">
        /// The LMS user id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUserParameters}"/>.
        /// </returns>
        public IFutureValue<LmsUserParameters> GetOneByAcIdCourseIdAndLmsUserId(string adobeConnectUserId, int lmsCourseId, int lmsUserId)
        {
            var queryOver =
                new DefaultQueryOver<LmsUserParameters, int>().GetQueryOver()
                .Where(x => x.AcId == adobeConnectUserId && x.Course == lmsCourseId && x.LmsUser.Id == lmsUserId).Take(1);
            return this.Repository.FindOne(queryOver);
        }
    }
}
