using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Core.Business.Queries;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace EdugameCloud.Core.Business.Models
{
    public class CompanyAcServerModel : BaseModel<CompanyAcServer, int>
    {
        private ICache _cache;


        public CompanyAcServerModel(IRepository<CompanyAcServer, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        //public override IEnumerable<CompanyAcServer> GetAll()
        //{
        //    return CacheUtility.GetCachedItem<List<CompanyAcServer>>(_cache, CachePolicies.Keys.CompanyAcServers(), () =>
        //    {
        //        var query = new DefaultQueryOver<CompanyAcServer, int>().GetQueryOver().OrderBy(x => x.Id).Asc;
        //        return this.Repository.FindAll(query).ToList();
        //    });
        //}


        public IEnumerable<CompanyAcServer> GetAllByCompany(int companyId)
        {
            var acServers = GetAll();
            var items = acServers.Where(x => x.Company.Id == companyId);
            return items;
        }


        public override void RegisterSave(CompanyAcServer entity)
        {
            base.RegisterSave(entity);
            // uncheck current default one if default true
        }
    }
}