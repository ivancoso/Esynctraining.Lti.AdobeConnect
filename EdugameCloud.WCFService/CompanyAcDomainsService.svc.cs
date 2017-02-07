using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;

namespace EdugameCloud.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompanyAcDomainsService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CompanyAcDomainsService.svc or CompanyAcDomainsService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CompanyAcDomainsService : BaseService, ICompanyAcDomainsService
    {
        public CompanyAcDomainDTO[] GetAllByCompany(int companyId)
        {
            var result = new List<CompanyAcDomainDTO>()
            {
                new CompanyAcDomainDTO()
                {
                    AcServer = "testurl",
                    Password = "test",
                    IsDefault = false,
                    Username = "usertest"
                }
            };
            return result.ToArray();
        }

        public int DeleteById(int id)
        {
            return id;
        }

        public CompanyAcDomainDTO Save(CompanyAcDomainDTO acDomain)
        {
            return acDomain;
        }
    }
}
