using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Sakai
{
    public interface ISakaiLmsApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            LmsCompany company,
            int courseid,
            out string error);

        bool LoginAndCheckSession(
            out string error,
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false);
    }
}