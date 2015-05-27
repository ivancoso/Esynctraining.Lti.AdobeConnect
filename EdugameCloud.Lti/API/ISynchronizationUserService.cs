using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API
{
    public interface ISynchronizationUserService
    {
        void SynchronizeUsers(LmsCompany lmsCompany, IEnumerable<string> scoIds = null);
    }
}