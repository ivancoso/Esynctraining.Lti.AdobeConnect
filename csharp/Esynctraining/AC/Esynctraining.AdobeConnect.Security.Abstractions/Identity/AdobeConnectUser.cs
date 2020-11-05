using System.Collections.Generic;

namespace Esynctraining.AdobeConnect.Security.Abstractions.Identity
{
    public class AdobeConnectUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string CompanyToken { get; set; }

        public string AcDomain { get; set; }

        public string AcSessionToken { get; set; }

        public IEnumerable<string> Roles { get; set; }

    }

}
