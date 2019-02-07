using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSetup
    {
        Task<OperationResult> SaveMeeting(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            ILtiParam param,
            MeetingDTOInput meetingDTO,
            StringBuilder trace,
            IFolderBuilder fb,
            bool retrieveLmsUsers = false);

        Task<(List<string> data, string error)> DeleteMeetingAsync(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            int id);
        
        MeetingSetup.LoginResult ACLogin(ILmsLicense lmsLicense,
            LtiParamDTO param, 
            LmsUser lmsUser,
            IAdobeConnectProxy provider);

    }

}
