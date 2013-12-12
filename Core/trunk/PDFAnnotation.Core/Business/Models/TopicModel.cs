namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Topic model class.
    /// </summary>
    public class TopicModel : BaseModel<Topic, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TopicModel(IRepository<Topic, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all by case id.
        /// </summary>
        /// <param name="caseId">
        /// The case id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Topic}"/>.
        /// </returns>
        public IEnumerable<Topic> GetAllByCategoryId(int caseId)
        {
            var defaultQuery = new DefaultQueryOver<Topic, int>().GetQueryOver().Where(x => x.Category.Id == caseId);
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by categories ids.
        /// </summary>
        /// <param name="casesIds">
        /// The categories ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Topic}"/>.
        /// </returns>
        public IEnumerable<Topic> GetAllByCasesIds(List<int> casesIds)
        {
            var query =
                new DefaultQueryOver<Topic, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Category.Id)
                    .IsIn(casesIds);
            return this.Repository.FindAll(query);
        }

        #endregion
    }
}