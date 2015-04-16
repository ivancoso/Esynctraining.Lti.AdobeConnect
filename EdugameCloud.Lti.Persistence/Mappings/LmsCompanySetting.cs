using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCompanySettingMap : BaseClassMap<LmsCompanySetting>
    {
        public LmsCompanySettingMap()
        {
            this.References(x => x.LmsCompany).Column("lmsCompanyId").Not.Nullable();
            this.Map(x => x.Name).Not.Nullable();
            this.Map(x => x.Value).Not.Nullable();
        }
    }
}
