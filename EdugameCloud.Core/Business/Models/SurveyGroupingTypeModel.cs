namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The SurveyGroupingType model.
    /// </summary>
    public class SurveyGroupingTypeModel : BaseModel<SurveyGroupingType, int>
    {
        private readonly ICache _cache;


        public SurveyGroupingTypeModel(IRepository<SurveyGroupingType, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public override IEnumerable<SurveyGroupingType> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<SurveyGroupingType>>(_cache, CachePolicies.Keys.SurveyGroupingTypes(), () =>
            {
                var query = new DefaultQueryOver<SurveyGroupingType, int>().GetQueryOver().OrderBy(x => x.SurveyGroupingTypeName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}