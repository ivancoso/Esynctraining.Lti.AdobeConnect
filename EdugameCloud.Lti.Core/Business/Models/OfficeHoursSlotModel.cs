using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate.SqlCommand;

namespace EdugameCloud.Lti.Core.Business.Models
{
    public sealed class OfficeHoursSlotModel : BaseModel<OfficeHoursSlot, int>
    {
        public OfficeHoursSlotModel(IRepository<OfficeHoursSlot, int> repository) : base(repository)
        {
        }

        public IEnumerable<OfficeHoursSlot> GetSlotsForDate(DateTime start, DateTime end, int ohId)
        {
            OfficeHoursTeacherAvailability availability = null;
            OfficeHoursSlot x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<OfficeHoursSlot, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.User, () => u, JoinType.InnerJoin)
                .JoinAlias(() => x.Availability, () => availability, JoinType.InnerJoin)
                .JoinAlias(() => availability.Meeting, () => oh, JoinType.InnerJoin)
                .Where(() => x.Start >= start && x.Start < end && oh.Id == ohId);
            return Repository.FindAll(defaultQuery);
        }

        public OfficeHoursSlot GetSlotForDate(DateTime start, int ohId)
        {
            OfficeHoursTeacherAvailability availability = null;
            OfficeHoursSlot x = null;
            OfficeHours oh = null;
            LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<OfficeHoursSlot, int>()
                .GetQueryOver(() => x)
                .JoinAlias(() => x.User, () => u, JoinType.InnerJoin)
                .JoinAlias(() => x.Availability, () => availability, JoinType.InnerJoin)
                .JoinAlias(() => availability.Meeting, () => oh, JoinType.InnerJoin)
                .Where(() => x.Start == start && oh.Id == ohId);
            return Repository.FindOne(defaultQuery).Value;
        }
    }
}