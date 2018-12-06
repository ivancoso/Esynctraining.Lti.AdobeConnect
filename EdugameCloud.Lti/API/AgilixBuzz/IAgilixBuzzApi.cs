using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AgilixBuzz
{
    public interface IAgilixBuzzApi
    {
        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(
            ILmsLicense company,
            int courseid,
            object session = null);

        Task<(bool result, string error)> LoginAndCheckSessionAsync(string lmsDomain, string userName, string password);

    }

}
