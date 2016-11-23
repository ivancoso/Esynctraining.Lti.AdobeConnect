using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public class AdobeConnectUser : IUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string CompanyToken { get; set; }

        public string AcDomain { get; set; }

        public string AcSessionToken { get; set; }

        public IEnumerable<string> Roles { get; set; }

    }

}
