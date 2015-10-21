namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The QuizFormat model.
    /// </summary>
    public class QuizFormatModel : BaseModel<QuizFormat, int>
    {
        private readonly ICache _cache;


        public QuizFormatModel(IRepository<QuizFormat, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public override IEnumerable<QuizFormat> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<QuizFormat>>(_cache, CachePolicies.Keys.QuizFormats(), () =>
            {
                var query = new DefaultQueryOver<QuizFormat, int>().GetQueryOver().OrderBy(x => x.QuizFormatName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}