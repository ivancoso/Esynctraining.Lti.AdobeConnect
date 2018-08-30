using System.Collections.Generic;
using Esynctraining.BlackBoardClient;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API.BlackBoard
{
    public interface IBlackBoardApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            Dictionary<string, object> licenseSettings,
            string courseid,
            string[] userIds,
            out string error,
            ref WebserviceWrapper client);

        WebserviceWrapper LoginToolAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string password);

        WebserviceWrapper LoginUserAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string userName,
            string password);

        bool TryRegisterEGCTool(string lmsDomain, string registrationPassword, string initialPassword, out string error);

        string[] CreateAnnouncement(string courseId, string userUuid, Dictionary<string, object> licenseSettings, string announcementTitle, string announcementMessage);
    }

}
