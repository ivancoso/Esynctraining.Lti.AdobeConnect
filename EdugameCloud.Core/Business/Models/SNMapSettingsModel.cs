namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    /// The SN Map settings model.
    /// </summary>
    public class SNMapSettingsModel : BaseModel<SNMapSettings, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapSettingsModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNMapSettingsModel(IRepository<SNMapSettings, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}