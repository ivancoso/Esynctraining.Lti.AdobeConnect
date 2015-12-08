namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    /// The SN Service model.
    /// </summary>
    public class SNServiceModel : BaseModel<SNService, int>
    {
        private readonly ICache _cache;


        public SNServiceModel(IRepository<SNService, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public override IEnumerable<SNService> GetAll()
        {
            return CacheUtility.GetCachedItem<List<SNService>>(_cache, CachePolicies.Keys.SNServices(), () =>
            {
                var query = new DefaultQueryOver<SNService, int>().GetQueryOver().OrderBy(x => x.SocialService).Asc;
                return this.Repository.FindAll(query).ToList();
            });
        }

    }

}