using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Domain;
using Esynctraining.BlackBoardClient;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.BlackBoard
{
    public class BlackboardLmsUserService : LmsUserServiceBase
    {
        private readonly IBlackBoardApi _soapApi;

        public BlackboardLmsUserService(ILogger logger, IBlackBoardApi soapApi) : base(logger)
        {
            _soapApi = soapApi ?? throw new ArgumentNullException(nameof(soapApi)); 
        }

        public override async Task<OperationResultWithData<LmsUserDTO>> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, ILtiParam extraData = null)
        {
            Guid guid;
            //return GetUsersOldStyle(lmsCompany, courseId, out error)
            //    .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id));

            var result = await GetUsers(licenseSettings, courseId, extraData);
            return result.IsSuccess
                ? result.Data
                    .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id))
                    .ToSuccessResult()
                : OperationResultWithData<LmsUserDTO>.Error(result.Message);
        }
        
        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings, string courseId, ILtiParam param = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            string error = null;
            string[] userIds = null;

            WebserviceWrapper client = null;
            List<LmsUserDTO> users = _soapApi.GetUsersForCourse(
                licenseSettings,
                courseId,
                userIds,
                out error,
                ref client);

            if ((users.Count == 0)
                && error.Return(
                    x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0,
                    false))
            {
                Logger.Warn("GetBlackBoardUsers.AccessDenied. " + error);

                if (client != null)
                    client.logout();
                // NOTE: set to null to re-create session.
                client = null;
                users = _soapApi.GetUsersForCourse(
                    licenseSettings,
                    courseId,
                    userIds,
                    out error,
                    ref client);
            }

            // NOTE: always call logout
            if (client != null)
                client.logout();

            return !string.IsNullOrEmpty(error)
                ? OperationResultWithData<List<LmsUserDTO>>.Error(error)
                : users.ToSuccessResult();
        }
        
    }

}