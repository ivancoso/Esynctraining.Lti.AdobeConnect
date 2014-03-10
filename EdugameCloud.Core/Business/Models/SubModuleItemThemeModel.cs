namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SubModule Item Theme model.
    /// </summary>
    public class SubModuleItemThemeModel : BaseModel<SubModuleItemTheme, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemThemeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SubModuleItemThemeModel(IRepository<SubModuleItemTheme, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}