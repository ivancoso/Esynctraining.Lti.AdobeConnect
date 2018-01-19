using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Domain;
using Esynctraining.BlackBoardClient;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.BlackBoard
{
    public class BlackboardLmsUserService : LmsUserServiceBase
    {
        private readonly IBlackBoardApi _soapApi;


        public BlackboardLmsUserService(ILogger logger, IBlackBoardApi soapApi) : base(logger)
        {
            _soapApi = soapApi ?? throw new ArgumentNullException(nameof(soapApi)); 
        }


        public override async Task<(LmsUserDTO user, string error)> GetUser(ILmsLicense lmsCompany, string lmsUserId, int courseId, LtiParamDTO extraData = null)
        {
            Guid guid;
            //return GetUsersOldStyle(lmsCompany, courseId, out error)
            //    .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id));

            var result = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            if (string.IsNullOrWhiteSpace(result.error))
                return (result.users.FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id)), result.error);

            return (null, result.error);
        }
        
        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.Item1.ToSuccessResult();
        }

        public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            string error = null;
            string[] userIds = null;

            WebserviceWrapper client = null;
            List<LmsUserDTO> users = _soapApi.GetUsersForCourse(
                lmsCompany,
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
                    lmsCompany,
                    courseId,
                    userIds,
                    out error,
                    ref client);
            }

            // NOTE: always call logout
            if (client != null)
                client.logout();
            
            return Task.FromResult<(List<LmsUserDTO> users, string error)>((GroupUsers(users), error));
        }
        
    }

}