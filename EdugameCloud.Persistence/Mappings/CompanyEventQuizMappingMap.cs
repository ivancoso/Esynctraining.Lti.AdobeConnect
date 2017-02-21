using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Persistence.Mappings
{
    public class CompanyEventQuizMappingMap : BaseClassMap<CompanyEventQuizMapping>
    {
        #region Constructors and Destructors

        public CompanyEventQuizMappingMap()
        {
            Table("CompanyEventQuizMapping");
            this.Map(x => x.AcEventScoId).Length(50);

            this.References(x => x.PreQuiz).ForeignKey("PreQuizId").Nullable();
            this.References(x => x.PostQuiz).ForeignKey("PostQuizId").Nullable();
            this.References(x => x.CompanyAcDomain).ForeignKey("CompanyAcDomainId");
        }

        #endregion
    }
}