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

        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyModel CompanyModel
        {
            get { return IoC.Resolve<CompanyModel>(); }
        }


        public ACDomainDTO[] GetAllByCompany(int companyId)
        {
            var items = CompanyAcServerModel.GetAllByCompany(companyId).Select(x => new ACDomainDTO()
            {
                //password = x.Password,
                isDefault = x.IsDefault,
                user = x.Username,
                path = x.AcServer,
                companyId = x.Company.Id,
                domainId = x.Id
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
            var companyAcServer = CompanyAcServerModel.GetOneById(id).Value;
            if (companyAcServer != null)
                CompanyAcServerModel.RegisterDelete(companyAcServer);
            
            return id;
        }

        public ACDomainDTO Save(ACDomainDTO acDomain)
        {
            if (acDomain.isDefault == true)
            {
                var defaultDomain = CompanyAcServerModel.GetAllByCompany(acDomain.companyId).FirstOrDefault(x => x.IsDefault);
                if (defaultDomain != null)
                {
                    defaultDomain.IsDefault = false;
                    CompanyAcServerModel.RegisterSave(defaultDomain, true);
                }
            }
            var company = CompanyModel.GetOneById(acDomain.companyId).Value;
            CompanyAcServer companyAcServer;
            if (acDomain.domainId != 0)
            {
                companyAcServer = CompanyAcServerModel.GetOneById(acDomain.domainId).Value;
            }
            else
            {
                companyAcServer = new CompanyAcServer()
                {

                };
            }
            companyAcServer.Company = company;
            companyAcServer.IsDefault = acDomain.isDefault;
            companyAcServer.Username = acDomain.user;
            companyAcServer.AcServer = acDomain.path;

            if (acDomain.password != null)
                companyAcServer.Password = acDomain.password;
            CompanyAcServerModel.RegisterSave(companyAcServer, true);
            return acDomain;
        }
    }
}
