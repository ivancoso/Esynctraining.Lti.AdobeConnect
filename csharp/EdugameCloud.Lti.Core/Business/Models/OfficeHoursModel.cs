using System;

namespace EdugameCloud.Lti.Core.Business.Models
{
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.NHibernate;
    using NHibernate;
    using NHibernate.Criterion;

    public sealed class OfficeHoursModel : BaseModel<OfficeHours, int>
    {
        public OfficeHoursModel(IRepository<OfficeHours, int> repository)
            : base(repository)
        {
        }

        public IFutureValue<OfficeHours> GetByLmsUserId(int lmsUserId)
        {
            QueryOver<OfficeHours, OfficeHours> queryOver = QueryOver.Of<OfficeHours>().Where(s => s.LmsUser.Id == lmsUserId);

            return this.Repository.FindOne(queryOver);
        }
    }
}
