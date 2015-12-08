using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface ICanvasAPI
    {
        AnnouncementDTO CreateAnnouncement(
            string api,
            string usertoken,
            int courseid,
            string title,
            string message);

        bool IsTokenExpired(string api, string usertoken);

        //void AddMoreDetailsForUser(string api, string usertoken, LmsUserDTO user);

        

        LmsUserDTO GetUser(string api, string userToken, string userId);

    }

}
