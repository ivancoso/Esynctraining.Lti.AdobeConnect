//using System.Collections.Generic;
//using System.Linq;
//using EdugameCloud.Core.Domain.Entities;
//using Esynctraining.Core.Caching;
//using Esynctraining.NHibernate;
//using Esynctraining.NHibernate.Queries;

//namespace EdugameCloud.Core.Business.Models
//{
//    public class SchoolModel : BaseModel<School, int>
//    {
//        private readonly ICache _cache;


//        public SchoolModel(IRepository<School, int> repository, ICache cache)
//            : base(repository)
//        {
//            _cache = cache;
//        }


//        public School GetOneByName(string schoolNumber)
//        {
//            return GetAll().SingleOrDefault(x => x.SchoolNumber.Equals(schoolNumber, System.StringComparison.InvariantCultureIgnoreCase));
//        }

//        public override IEnumerable<School> GetAll()
//        {
//            return CacheUtility.GetCachedItem<List<School>>(_cache, CachePolicies.Keys.Schools(), () =>
//            {
//                var query = new DefaultQueryOver<School, int>().GetQueryOver().OrderBy(x => x.SchoolNumber).Asc;
//                return this.Repository.FindAll(query).ToList();
//            });
//        }

//    }
//}