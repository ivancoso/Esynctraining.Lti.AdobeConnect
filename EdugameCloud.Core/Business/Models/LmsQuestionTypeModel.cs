namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    /// <summary>
    /// The lms question type model.
    /// </summary>
    public class LmsQuestionTypeModel : BaseModel<LmsQuestionType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuestionTypeModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsQuestionTypeModel(IRepository<LmsQuestionType, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all active.
        /// </summary>
        /// <param name="lmsProviderId">
        /// The lms Provider Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionType}"/>.
        /// </returns>
        public IEnumerable<LmsQuestionType> GetAllByProvider(int lmsProviderId)
        {
            var query = new DefaultQueryOver<LmsQuestionType, int>().GetQueryOver().Where(x => x.LmsProvider.Id == lmsProviderId);
            return this.Repository.FindAll(query);
        }
    }
}
