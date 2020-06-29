using Esynctraining.BlackBoardClient;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.BlackBoard
{
    public class BlackboardLmsUserService : LmsUserServiceBase
    {
        private readonly IBlackBoardApi _soapApi;


        public BlackboardLmsUserService(ILogger logger, IBlackBoardApi soapApi) : base(logger)
        {
            _soapApi = soapApi ?? throw new ArgumentNullException(nameof(soapApi));
        }


        //public override async Task<(LmsUserDTO user, string error)> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, LtiParamDTO extraData = null)
        //{
        //    Guid guid;
        //    //return GetUsersOldStyle(lmsCompany, courseId, out error)
        //    //    .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id));

        //    var result = await GetUsersOldStyle(licenseSettings, courseId, extraData);
        //    if (string.IsNullOrWhiteSpace(result.error))
        //        return (result.users.FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id)), result.error);

        //    return (null, result.error);
        //}

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiParam param = null)
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

            if ((users.Count == 0) && (error == null ? false : error.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0))
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

            return string.IsNullOrEmpty(error)
                ? GroupUsers(users).ToSuccessResult()
                : OperationResultWithData<List<LmsUserDTO>>.Error(error);
        }

    }
}
