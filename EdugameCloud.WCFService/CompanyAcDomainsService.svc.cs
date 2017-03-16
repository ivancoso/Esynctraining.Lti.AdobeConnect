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

            var companyId = 0;
            if (companyAcServer != null)
            {
                companyId = companyAcServer.Company.Id;
                CompanyAcServerModel.RegisterDelete(companyAcServer, true);
            }

            // if default is deleted - set first one as default
            if (!CompanyAcServerModel.GetAllByCompany(companyId).Any(x => x.IsDefault))
            {
                var defaultDomain = CompanyAcServerModel.GetAllByCompany(companyId).FirstOrDefault();
                if (defaultDomain != null)
                {
                    defaultDomain.IsDefault = true;
                    CompanyAcServerModel.RegisterSave(defaultDomain, true);
                }
            }
           
            return id;
        }

        public ACDomainDTO Save(ACDomainDTO acDomain)
        {
            if (acDomain.isDefault)
            {
                var defaultDomain =
                    CompanyAcServerModel.GetAllByCompany(acDomain.companyId).FirstOrDefault(x => x.IsDefault);
                if (defaultDomain != null)
                {
                    defaultDomain.IsDefault = false;
                    CompanyAcServerModel.RegisterSave(defaultDomain, true);
                }
            }
            else
            {
                // for update
                if (acDomain.domainId != 0)
                {
                    // you can't uncheck default if there is only one record
                    if (CompanyAcServerModel.GetAllByCompany(acDomain.companyId).Count() == 1)
                        return acDomain;

                    if (CompanyAcServerModel.GetAllByCompany(acDomain.companyId).Count() > 1)
                    {
                        var firstAcServer =
                            CompanyAcServerModel.GetAllByCompany(acDomain.companyId)
                                .FirstOrDefault(x => x.Id != acDomain.domainId);
                        if (firstAcServer != null)
                        {
                            firstAcServer.IsDefault = true;
                            CompanyAcServerModel.RegisterSave(firstAcServer, true);
                        }
                    }
                }
                else
                {
                    var existing = CompanyAcServerModel.GetAllByCompany(acDomain.companyId).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.IsDefault = true;
                        CompanyAcServerModel.RegisterSave(existing, true);
                    }
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

            if (!string.IsNullOrEmpty(acDomain.password))
                companyAcServer.Password = acDomain.password;
            CompanyAcServerModel.RegisterSave(companyAcServer, true);
            return acDomain;
        }

        public ACDomainDTO GetById(int id)
        {
            var companyAcServer = CompanyAcServerModel.GetOneById(id).Value;
            var result = new ACDomainDTO()
            {
                companyId = companyAcServer.Company.Id,
                password = companyAcServer.Password,
                domainId = companyAcServer.Id,
                isDefault = companyAcServer.IsDefault,
                path = companyAcServer.AcServer,
                user = companyAcServer.Username
            };
            return result;
        }
    }
}
