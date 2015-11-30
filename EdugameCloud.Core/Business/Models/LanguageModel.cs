namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    /// The language model.
    /// </summary>
    public class LanguageModel : BaseModel<Language, int>
    {
        private readonly ICache _cache;

        #region Constructors and Destructors
        
        public LanguageModel(IRepository<Language, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        #endregion

        public override IEnumerable<Language> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<Language>>(_cache, CachePolicies.Keys.Languages(), () =>
            {
                var query = new DefaultQueryOver<Language, int>().GetQueryOver().OrderBy(x => x.LanguageName).Asc;
                return this.Repository.FindAll(query);
            });
        }

        public Language GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id");

            return GetAll().Single(x => x.Id == id);
        }

    }

}