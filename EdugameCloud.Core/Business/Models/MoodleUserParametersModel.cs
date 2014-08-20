namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    /// The moodle user parameters model
    /// </summary>
    public class MoodleUserParametersModel : BaseModel<MoodleUserParameters, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserParametersModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public MoodleUserParametersModel(IRepository<MoodleUserParameters, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Constructors and Destructors

        public IFutureValue<MoodleUserParameters> GetOneByAcId(string id)
        {
            var queryOver = new DefaultQueryOver<MoodleUserParameters, int>().GetQueryOver().Where(x => x.AcId == id);
            return this.Repository.FindOne(queryOver);
        }

        #endregion
    }
}
