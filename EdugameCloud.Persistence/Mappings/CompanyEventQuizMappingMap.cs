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

            this.References(x => x.PreQuiz).Column("PreQuizId").Nullable();
            this.References(x => x.PostQuiz).Column("PostQuizId").Nullable();
            this.References(x => x.CompanyAcDomain).Column("CompanyAcDomainId");
        }

        #endregion
    }
}