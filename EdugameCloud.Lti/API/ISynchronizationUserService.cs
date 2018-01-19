using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API
{
    public interface ISynchronizationUserService
    {
        Task SynchronizeUsers(ILmsLicense lmsCompany, bool syncACUsers, IEnumerable<int> meetingIds = null);

    }

}