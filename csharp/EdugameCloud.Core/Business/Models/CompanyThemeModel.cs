namespace EdugameCloud.Core.Business.Models
{
    using System;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class CompanyThemeModel : BaseModel<CompanyTheme, Guid>
    {
        public CompanyThemeModel(IRepository<CompanyTheme, Guid> repository)
            : base(repository)
        {
        }

    }

}