namespace EdugameCloud.Core.Business.Models
{
    using System;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The Company Theme model.
    /// </summary>
    public class CompanyThemeModel : BaseModel<CompanyTheme, Guid>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyThemeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyThemeModel(IRepository<CompanyTheme, Guid> repository)
            : base(repository)
        {
        }

        #endregion
    }
}