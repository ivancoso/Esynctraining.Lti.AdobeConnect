namespace EdugameCloud.Lti.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Business;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Caching;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    /// The LMS question type model.
    /// </summary>
    public sealed class LmsQuestionTypeModel : BaseModel<LmsQuestionType, int>
    {
        private readonly ICache _cache;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuestionTypeModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsQuestionTypeModel(IRepository<LmsQuestionType, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }

        #endregion

        /// <summary>
        /// The get all active.
        /// </summary>
        /// <param name="lmsProviderId">
        /// The LMS Provider Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionType}"/>.
        /// </returns>
        public IEnumerable<LmsQuestionType> GetAllByProvider(int lmsProviderId)
        {
            return GetAll().Where(x => x.LmsProvider.Id == lmsProviderId);
        }

        public override IEnumerable<LmsQuestionType> GetAll()
        {
            return CacheUtility.GetCachedItem(_cache, CachePolicies.Keys.LmsQuestionTypes(), () =>
            {
                var query = new DefaultQueryOver<LmsQuestionType, int>().GetQueryOver();
                return this.Repository.FindAll(query).ToList();
            });
        }

    }

}
