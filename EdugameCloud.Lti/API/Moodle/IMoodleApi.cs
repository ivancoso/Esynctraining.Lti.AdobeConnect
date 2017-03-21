using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Moodle
{
    public interface IMoodleApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            ILmsLicense company,
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
