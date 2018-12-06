using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using Esynctraining.Lti.Lms.Common.Dto;
using NHibernate;
using NHibernate.Linq;

namespace EdugameCloud.Lti.Core.Business.Models
{
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
            if (userIds == null)
                throw new ArgumentNullException("userIds");

            if (userIds.Length == 0)
                return Enumerable.Empty<LmsUser>();

            var result = new List<LmsUser>();
            //{"The incoming request has too many parameters. The server supports a maximum of 2100 parameters. Reduce the number of parameters and resend the request."}
            foreach (var chunk in userIds.Chunk(2000))
            {
                var query = from u in this.Repository.Session.Query<LmsUser>()
                            where u.LmsCompany.Id == companyLmsId && chunk.Contains(u.UserId)
                            select u;

                result.AddRange(query.ToList());
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
                    ((string.IsNullOrEmpty(u.LtiId) ? u.Id : u.LtiId) ?? string.Empty).Trim(),
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
