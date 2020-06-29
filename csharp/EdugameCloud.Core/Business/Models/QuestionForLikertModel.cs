namespace EdugameCloud.Core.Business.Models
{
    using System.Diagnostics.CodeAnalysis;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class QuestionForLikertModel : BaseModel<QuestionForLikert, int>
    {
        public QuestionForLikertModel(IRepository<QuestionForLikert, int> repository)
            : base(repository)
        {
        }

    }

}