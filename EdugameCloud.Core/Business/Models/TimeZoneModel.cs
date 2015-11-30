namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    /// The TimeZone model.
    /// </summary>
    public sealed class TimeZoneModel : BaseModel<TimeZone, int>
    {
        private readonly ICache _cache;
        

        public TimeZoneModel(IRepository<TimeZone, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public override IEnumerable<TimeZone> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<TimeZone>>(_cache, CachePolicies.Keys.TimeZones(), () =>
            {
                var query = new DefaultQueryOver<TimeZone, int>().GetQueryOver().OrderBy(x => x.TimeZoneName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}