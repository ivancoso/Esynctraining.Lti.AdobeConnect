namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Business;
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    /// The LMS provider model.
    /// </summary>
    public sealed class LmsProviderModel : BaseModel<LmsProvider, int>
    {
        private readonly ICache _cache;

        #region Constructors and Destructors
        
        public LmsProviderModel(IRepository<LmsProvider, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        #endregion

        // TRICK: uses cache!
        public LmsProvider GetByName(string name)
        {
            return GetAll().SingleOrDefault(x => x.ShortName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // TRICK: uses cache!
        public LmsProvider GetById(int lmsProviderId)
        {
            return GetAll().SingleOrDefault(x => x.Id == lmsProviderId);
        }

        // TRICK: uses cache!
        public override IEnumerable<LmsProvider> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<LmsProvider>>(_cache, CachePolicies.Keys.LmsProviders(), () =>
            {
                var queryOver = new DefaultQueryOver<LmsProvider, int>().GetQueryOver();
                return Repository.FindAll(queryOver).ToList();
            });
        }

    }

}
