using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSetup
    {
        Task<OperationResult> SaveMeeting(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            MeetingDTOInput meetingDTO,
            StringBuilder trace,
            IFolderBuilder fb,
            bool retrieveLmsUsers = false);

        Task<(List<string> data, string error)> DeleteMeetingAsync(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id);
        
        MeetingSetup.LoginResult ACLogin(ILmsLicense lmsCompany,
            LtiParamDTO param, 
            LmsUser lmsUser,
            IAdobeConnectProxy provider);

    }

}
