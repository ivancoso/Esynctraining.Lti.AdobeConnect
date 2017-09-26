using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Sakai
{
    public interface ISakaiLmsApi
    {
        Task<(List<LmsUserDTO> Data, string Error)> GetUsersForCourseAsync(
            LmsCompany company,
            int courseid);

        Task<(bool Data, string Error)> LoginAndCheckSessionAsync(
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false);
    }
}