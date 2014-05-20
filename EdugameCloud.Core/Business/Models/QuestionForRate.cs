﻿namespace EdugameCloud.Core.Business.Models
{
    using System.Diagnostics.CodeAnalysis;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The QuestionForLikert model.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class QuestionForRateModel : BaseModel<QuestionForRate, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForRateModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuestionForRateModel(IRepository<QuestionForRate, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}