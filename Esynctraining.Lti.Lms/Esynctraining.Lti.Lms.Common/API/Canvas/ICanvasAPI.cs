using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface ICanvasAPI
    {
        AnnouncementDTO CreateAnnouncement(
            string api,
            string userToken,
            string courseId,
            string title,
            string message);

        bool IsTokenExpired(string api, string userToken);

        LmsUserDTO GetUser(string api, string userToken, string userId);

    }

}
