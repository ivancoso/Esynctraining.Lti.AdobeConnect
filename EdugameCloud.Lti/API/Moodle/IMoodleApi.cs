using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.API.Moodle
{
    public interface IMoodleApi
    {
        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourse(ILmsLicense company, int courseId);

        Task<(bool result, string error)> LoginAndCheckSession(
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false);

    }

}
