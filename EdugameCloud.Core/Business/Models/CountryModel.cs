namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The country model.
    /// </summary>
    public sealed class CountryModel : BaseModel<Country, int>
    {
        private readonly ICache _cache;
        

        public CountryModel(IRepository<Country, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public Country GetOneByName(string country)
        {
            return GetAll().SingleOrDefault(x => x.CountryName.Equals(country, System.StringComparison.InvariantCultureIgnoreCase));
        }
        
        public override IEnumerable<Country> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<Country>>(_cache, CachePolicies.Keys.Countries(), () =>
            {
                var query = new DefaultQueryOver<Country, int>().GetQueryOver().OrderBy(x => x.CountryName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}