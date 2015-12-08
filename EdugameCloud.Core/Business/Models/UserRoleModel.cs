namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    /// The user role model.
    /// </summary>
    public sealed class UserRoleModel : BaseModel<UserRole, int>
    {
        private readonly ICache _cache;
        

        public UserRoleModel(IRepository<UserRole, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }
        

        public override IEnumerable<UserRole> GetAll()
        {
            return CacheUtility.GetCachedItem<List<UserRole>>(_cache, CachePolicies.Keys.UserRoles(), () =>
            {
                var query = new DefaultQueryOver<UserRole, int>().GetQueryOver().OrderBy(x => x.UserRoleName).Asc;
                return this.Repository.FindAll(query).ToList();
            });
        }

    }

}