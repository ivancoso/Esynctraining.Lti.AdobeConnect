using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.AgilixBuzz
{
    public interface IAgilixBuzzApi
    {
        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(Dictionary<string, object> licenseSettings, string courseid, object session = null);

        Task<bool> LoginAndCheckSessionAsync(string lmsDomain, string userName, string password);

        Task<(LmsUserDTO result, string error)> GetEnrollmentAsync(Dictionary<string, object> licenseSettings, string enrollmentId, object extraData = null);

        Task<LmsUserDTO> GetUserAsync(Dictionary<string, object> licenseSettings, string lmsUserId, object extraData = null);
    }

}
