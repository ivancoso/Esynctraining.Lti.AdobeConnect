namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SurveyGroupingType model.
    /// </summary>
    public class SurveyGroupingTypeModel : BaseModel<SurveyGroupingType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyGroupingTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SurveyGroupingTypeModel(IRepository<SurveyGroupingType, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}