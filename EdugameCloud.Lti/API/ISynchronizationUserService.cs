using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API
{
    public interface ISynchronizationUserService
    {
        void SynchronizeUsers(LmsCompany lmsCompany, bool syncACUsers, IEnumerable<int> meetingIds = null);
    }
}