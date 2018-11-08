using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate.SqlCommand;

namespace EdugameCloud.Lti.Core.Business.Models
{
    public sealed class OfficeHoursTeacherAvailabilityModel : BaseModel<OfficeHoursTeacherAvailability, int>
    {
        public OfficeHoursTeacherAvailabilityModel(IRepository<OfficeHoursTeacherAvailability, int> repository) : base(repository)
        {
        }

        public IEnumerable<OfficeHoursTeacherAvailability> GetAvailabilities(int ohId)
        {
            OfficeHoursTeacherAvailability x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<OfficeHoursTeacherAvailability, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.Meeting, () => oh, JoinType.InnerJoin)
                .JoinAlias(() => x.User, () => u, JoinType.InnerJoin)
                .Where(() => x.Meeting.Id == ohId);
            return Repository.FindAll(defaultQuery);
        }
    }
}