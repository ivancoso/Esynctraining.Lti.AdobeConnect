namespace EdugameCloud.Core.Business.Models
{
    using System.Diagnostics.CodeAnalysis;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    /// The QuestionForLikert model.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class QuestionForLikertModel : BaseModel<QuestionForLikert, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForLikertModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuestionForLikertModel(IRepository<QuestionForLikert, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}