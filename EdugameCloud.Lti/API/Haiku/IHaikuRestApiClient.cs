using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Haiku
{
    public interface IHaikuRestApiClient
    {
        bool TestOauth(string lmsDomain, string consumerKey, string consumerSecret, string token, string tokenSecret);

        List<LmsUserDTO> GetUsersForCourse(ILmsLicense lmsCompany, int courseId, out string error);
    }
}
