namespace EdugameCloud.Core.Business.Models
{
    using System.Diagnostics.CodeAnalysis;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class QuestionForRateModel : BaseModel<QuestionForRate, int>
    {
        public QuestionForRateModel(IRepository<QuestionForRate, int> repository)
            : base(repository)
        {
        }

    }

}