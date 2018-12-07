using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.Haiku
{
    public interface IHaikuRestApiClient
    {
        Task<bool> TestOauthAsync(string lmsDomain, string consumerKey, string consumerSecret, string token, string tokenSecret);

        Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(Dictionary<string, object> licenseSettings, string courseId);

        Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(Dictionary<string, object> licenseSettings, string courseId);

    }

}
