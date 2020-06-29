using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.Moodle
{
    public interface IMoodleApi
    {
        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourse(Dictionary<string, object> licenseSettings, string courseId);

        Task<(bool result, string error)> LoginAndCheckSession(
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false);

    }

}
