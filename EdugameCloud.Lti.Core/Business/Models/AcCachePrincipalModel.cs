using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.NHibernate;
using NHibernate.Linq;

namespace EdugameCloud.Lti.Core.Business.Models
{
    public partial class AcCachePrincipalModel : BaseModel<AcCachePrincipal, int>
    {
        #region Constructor

        public AcCachePrincipalModel(IRepository<AcCachePrincipal, int> repository)
            : base(repository) { }

        #endregion

        public IEnumerable<AcCachePrincipal> GetByLmsCompany(int lmsCompanyId, List<LmsUserDTO> users)
        {
            // TODO: add parameter checks

            var emails = users.Select(x => x.GetEmail()).Where(email => !string.IsNullOrWhiteSpace(email)).Distinct().ToList();
            var logins = users.Select(x => x.GetLogin()).Where(login => !string.IsNullOrWhiteSpace(login)).Distinct().ToList();

            var query = from c in this.Repository.Session.Query<AcCachePrincipal>()
                        where c.LmsCompanyId == lmsCompanyId && (emails.Contains(c.Email) || logins.Contains(c.Login))
                        select c;

            return query.ToList();
        }

    }

}
