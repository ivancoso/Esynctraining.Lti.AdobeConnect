using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.BrainHoney
{
    public interface IBrainHoneyApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            LmsCompany company,
            int courseid,
            out string error,
            object session = null);

        bool LoginAndCheckSession(out string error, string lmsDomain, string userName, string password);

    }

}
