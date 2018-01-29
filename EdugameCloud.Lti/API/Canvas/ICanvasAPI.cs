using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface ICanvasAPI
    {
        AnnouncementDTO CreateAnnouncement(
            string api,
            string userToken,
            int courseId,
            string title,
            string message);

        bool IsTokenExpired(string api, string userToken);

        LmsUserDTO GetUser(string api, string userToken, string userId);

    }

}
