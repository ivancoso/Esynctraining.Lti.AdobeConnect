using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCompanyRoleMappingMap : BaseClassMap<LmsCompanyRoleMapping>
    {
        public LmsCompanyRoleMappingMap()
        {
            this.References(x => x.LmsCompany).Column("lmsCompanyId").Not.Nullable();
            this.Map(x => x.LmsRoleName).Not.Nullable();
            this.Map(x => x.AcRole).Not.Nullable();
            this.Map(x => x.IsDefaultLmsRole).Not.Nullable();
            this.Map(x => x.IsTeacherRole).Not.Nullable();
        }
    }
}
