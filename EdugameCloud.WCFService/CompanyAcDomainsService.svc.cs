using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;
using Esynctraining.Core.Utils;

namespace EdugameCloud.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompanyAcDomainsService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CompanyAcDomainsService.svc or CompanyAcDomainsService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CompanyAcDomainsService : BaseService, ICompanyAcDomainsService
    {
        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyAcServerModel CompanyAcServerModel
        {
            get { return IoC.Resolve<CompanyAcServerModel>(); }
        }


        public CompanyAcDomainDTO[] GetAllByCompany(int companyId)
        {
            var items = CompanyAcServerModel.GetAllByCompany(companyId).Select(x => new CompanyAcDomainDTO()
            {
                Password = x.Password,
                IsDefault = x.IsDefault,
                Username = x.Username,
                AcServer = x.AcServer,
                CompanyId = x.CompanyId
            });
            //var result = new List<CompanyAcDomainDTO>()
            //{
            //    new CompanyAcDomainDTO()
            //    {
            //        AcServer = "testurl",
            //        Password = "test",
            //        IsDefault = false,
            //        Username = "usertest"
            //    }
            //};
            return items.ToArray();
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
