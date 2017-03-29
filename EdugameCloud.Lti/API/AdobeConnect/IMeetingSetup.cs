using System.Collections.Generic;
using System.Text;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSetup
    {
        OperationResult SaveMeeting(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            MeetingDTOInput meetingDTO,
            StringBuilder trace,
            IFolderBuilder fb,
            bool retrieveLmsUsers = false);

        List<string> DeleteMeeting(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id,
            out string error);
        
        MeetingSetup.LoginResult ACLogin(ILmsLicense lmsCompany,
            LtiParamDTO param, 
            LmsUser lmsUser,
            IAdobeConnectProxy provider);

    }

}
