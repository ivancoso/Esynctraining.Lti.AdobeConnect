using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Haiku
{
    public interface IHaikuRestApiClient
    {
        Task<bool> TestOauthAsync(string lmsDomain, string consumerKey, string consumerSecret, string token, string tokenSecret);

        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(ILmsLicense lmsCompany, int courseId);

        Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(ILmsLicense lmsCompany, int courseId);

    }

}
