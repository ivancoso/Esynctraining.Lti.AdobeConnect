using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IPrincipalCache
    {
        void RecreatePrincipalCache(IEnumerable<LmsCompany> acDomains);

    }

}
