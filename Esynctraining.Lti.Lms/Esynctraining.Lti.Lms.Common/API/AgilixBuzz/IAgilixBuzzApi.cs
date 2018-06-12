using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.AgilixBuzz
{
    public interface IAgilixBuzzApi
    {
        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(
            Dictionary<string, object> licenseSettings,
            int courseid,
            object session = null);

        Task<(bool result, string error)> LoginAndCheckSessionAsync(string lmsDomain, string userName, string password);

    }

}
