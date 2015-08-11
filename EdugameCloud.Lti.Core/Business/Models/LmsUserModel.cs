namespace EdugameCloud.Lti.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using NHibernate;
    using NHibernate.Linq;

    /// <summary>
    /// The LMS user model
    /// </summary>
    public sealed class LmsUserModel : BaseModel<LmsUser, int>
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
        /// The company LMS Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUser}"/>.
        /// </returns>
        public IFutureValue<LmsUser> GetOneByUserIdAndCompanyLms(string userId, int companyLmsId)
        {
            var queryOver = new DefaultQueryOver<LmsUser, int>()
                .GetQueryOver()
                .Where(u => u.LmsCompany.Id == companyLmsId && u.UserId == userId);
            return this.Repository.FindOne(queryOver);
        }

        public IEnumerable<LmsUser> GetByUserIdAndCompanyLms(string[] userIds, int companyLmsId)
        {
            var query = from u in this.Repository.Session.Query<LmsUser>()
                        where u.LmsCompany.Id == companyLmsId && userIds.Contains(u.UserId)
                        select u;

            return query.ToList().AsReadOnly();
        }

        /// <summary>
        /// The get one by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="userLogin">
        /// The user Login.
        /// </param>
        /// <param name="userEmail">
        /// The user Email.
        /// </param>
        /// <param name="companyLmsId">
        /// The company LMS Id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsUser}"/>.
        /// </returns>
        public LmsUser GetOneByUserIdOrUserNameOrEmailAndCompanyLms(string userId, string userLogin, string userEmail, int companyLmsId)
        {
            var queryOver = new DefaultQueryOver<LmsUser, int>().GetQueryOver()
                .Where(u => (u.LmsCompany.Id == companyLmsId) && (u.UserId == userId || u.Username == userLogin || u.Username == userEmail));
            var result = this.Repository.FindOne(queryOver).Value;
            if (result != null && !string.IsNullOrWhiteSpace(userId) && !userId.Equals(result.UserId))
            {
                result.UserId = userId;
                this.RegisterSave(result);
            }

            return result;
        }

        public IEnumerable<LmsUser> GetByCompanyLms(int companyLmsId, IList<LmsUserDTO> usersToFind)
        {
            var lmsUserSelectParam = new StringBuilder(50 * usersToFind.Count);
            lmsUserSelectParam.Append("<users>");
            foreach (LmsUserDTO u in usersToFind)
            {

                lmsUserSelectParam.AppendFormat("<user id=\"{0}\" email=\"{1}\" login=\"{2}\" />",
                    ((string.IsNullOrEmpty(u.lti_id) ? u.id : u.lti_id) ?? string.Empty).Trim(),
                    u.GetEmail(),
                    u.GetLogin());
            }
            lmsUserSelectParam.Append("</users>");

            var query = this.Repository.Session.GetNamedQuery("getUsersByLmsCompanyId");
            query.SetParameter("lmsCompanyId", companyLmsId);
            query.SetParameter("userFilter", lmsUserSelectParam.ToString(), NHibernateUtil.StringClob);
            return query.List<LmsUser>();
        }

        #endregion

    }

}
