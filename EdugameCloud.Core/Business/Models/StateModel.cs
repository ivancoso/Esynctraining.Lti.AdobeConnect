namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;
    using NHibernate;

    /// <summary>
    /// The state model.
    /// </summary>
    public class StateModel : BaseModel<State, int>
    {
        private readonly ICache _cache;


        public StateModel(IRepository<State, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public State GetOneByName(string stateName)
        {
            return GetAll().SingleOrDefault(x => x.StateName.Equals(stateName, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public State GetOneByCode(string stateCode)
        {
            return GetAll().SingleOrDefault(x => x.StateCode.Equals(stateCode, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public override IEnumerable<State> GetAll()
        {
            return CacheUtility.GetCachedItem<IEnumerable<State>>(_cache, CachePolicies.Keys.States(), () =>
            {
                var query = new DefaultQueryOver<State, int>().GetQueryOver().OrderBy(x => x.StateName).Asc;
                return this.Repository.FindAll(query);
            });
        }

    }

}