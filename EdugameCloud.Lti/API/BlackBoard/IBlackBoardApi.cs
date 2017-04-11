using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.BlackBoardClient;

namespace EdugameCloud.Lti.API.BlackBoard
{
    public interface IBlackBoardApi
    {
        List<LmsUserDTO> GetUsersForCourse(
            ILmsLicense company,
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

        string[] CreateAnnouncement(int courseId, string userUuid, ILmsLicense lmsCompany, string announcementTitle, string announcementMessage);
    }

}
