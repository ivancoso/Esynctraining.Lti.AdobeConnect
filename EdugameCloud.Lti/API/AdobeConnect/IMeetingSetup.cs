using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSetup
    {
        void SetupFolders(LmsCompany credentials, IAdobeConnectProxy provider);

        OperationResult SaveMeeting(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            MeetingDTO meetingDTO,
            bool retrieveLmsUsers = false);

        List<string> DeleteMeeting(
            LmsCompany credentials,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id,
            out string error);

        List<TemplateDTO> GetTemplates(IAdobeConnectProxy provider, string templateFolder);

        IAdobeConnectProxy GetProvider(LmsCompany credentials, bool login = true);

        string ACLogin(LmsCompany lmsCompany, LtiParamDTO param, LmsUser lmsUser,
            Principal registeredUser, IAdobeConnectProxy provider);
    }

}
