namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The ScoreType model.
    /// </summary>
    public sealed class ScoreTypeModel : BaseModel<ScoreType, int>
    {
        private readonly ICache _cache;


        public ScoreTypeModel(IRepository<ScoreType, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        public override IEnumerable<ScoreType> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<ScoreType>>(_cache, CachePolicies.Keys.ScoreTypes(), () =>
            {
                var query = new DefaultQueryOver<ScoreType, int>().GetQueryOver().OrderBy(x => x.ScoreTypeName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}