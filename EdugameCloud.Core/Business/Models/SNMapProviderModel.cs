namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The SN Map provider model.
    /// </summary>
    public class SNMapProviderModel : BaseModel<SNMapProvider, int>
    {
        private readonly ICache _cache;


        public SNMapProviderModel(IRepository<SNMapProvider, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public override IEnumerable<SNMapProvider> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<SNMapProvider>>(_cache, CachePolicies.Keys.SNMapProviders(), () =>
            {
                var query = new DefaultQueryOver<SNMapProvider, int>().GetQueryOver().OrderBy(x => x.MapProvider).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}