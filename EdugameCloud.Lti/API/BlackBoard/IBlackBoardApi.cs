using System.Collections.Generic;
using BbWsClient;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.BlackBoard
{
    public interface IBlackBoardApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            LmsCompany company,
            int courseid,
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

        string[] CreateAnnouncement(int courseId, string userUuid, LmsCompany lmsCompany, string announcementTitle, string announcementMessage);
    }

}
