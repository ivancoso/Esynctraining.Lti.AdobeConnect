namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    /// <summary>
    ///     The QuestionType model.
    /// </summary>
    public class QuestionTypeModel : BaseModel<QuestionType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuestionTypeModel(IRepository<QuestionType, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all active.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionType}"/>.
        /// </returns>
        public IEnumerable<QuestionType> GetAllActive()
        {
            var query = new DefaultQueryOver<QuestionType, int>().GetQueryOver().Where(x => x.IsActive == true);
            return this.Repository.FindAll(query);
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