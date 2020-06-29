namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class SubModuleItemThemeModel : BaseModel<SubModuleItemTheme, int>
    {
        public SubModuleItemThemeModel(IRepository<SubModuleItemTheme, int> repository)
            : base(repository)
        {
        }

    }

}