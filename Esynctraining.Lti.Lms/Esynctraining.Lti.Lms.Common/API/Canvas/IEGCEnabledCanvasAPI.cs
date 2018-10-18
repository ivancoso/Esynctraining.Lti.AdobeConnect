using Esynctraining.Lti.Lms.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string courseId);
        Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string courseid, Dictionary<string, object> licenseSettings);
    }

}
