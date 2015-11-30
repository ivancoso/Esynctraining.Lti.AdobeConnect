using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate;

namespace EdugameCloud.Lti.Core.Business.Models
{
    public sealed class LmsCompanySettingModel : BaseModel<LmsCompanySetting, int>
    {
        public LmsCompanySettingModel(IRepository<LmsCompanySetting, int> repository) : base(repository)
        {
        }

        public IFutureValue<LmsCompanySetting> GetOneByLmsCompanyIdAndSettingName(int lmsCompanyId, string settingName)
        {
            var defaultQuery = new DefaultQueryOver<LmsCompanySetting, int>().GetQueryOver()
                .Where(x => x.LmsCompany.Id == lmsCompanyId && x.Name == settingName)
                .Take(1);
            return this.Repository.FindOne(defaultQuery);
        }
    }
}