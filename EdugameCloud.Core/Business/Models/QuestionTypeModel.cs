namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    public class QuestionTypeModel : BaseModel<QuestionType, int>
    {
        private readonly ICache _cache;
        
        
        public QuestionTypeModel(IRepository<QuestionType, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public IEnumerable<QuestionType> GetAllActive()
        {
            return GetAll().Where(x => x.IsActive == true);
        }

        public override IEnumerable<QuestionType> GetAll()
        {
            return CacheUtility.GetCachedItem<List<QuestionType>>(_cache, CachePolicies.Keys.QuestionTypes(), () =>
            {
                var query = new DefaultQueryOver<QuestionType, int>().GetQueryOver().OrderBy(x => x.Type).Asc;
                return this.Repository.FindAll(query).ToList();
            });
        }

    }

}