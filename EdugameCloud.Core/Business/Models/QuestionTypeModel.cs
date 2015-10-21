namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Caching;

    /// <summary>
    ///     The QuestionType model.
    /// </summary>
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
            return CacheUtility.GetCachedItem<IEnumerable<QuestionType>>(_cache, CachePolicies.Keys.QuestionTypes(), () =>
            {
                var query = new DefaultQueryOver<QuestionType, int>().GetQueryOver().OrderBy(x => x.Type).Asc;
                return this.Repository.FindAll(query);
            });
        }


        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page items.
        /// </param>
        /// <param name="totalCount">
        /// The total Count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionType}"/>.
        /// </returns>
        public IEnumerable<QuestionType> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<QuestionType, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }
    }
}