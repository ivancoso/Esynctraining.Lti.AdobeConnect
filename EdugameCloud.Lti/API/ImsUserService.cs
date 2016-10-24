using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API
{
    public class ImsUserService : LmsUserServiceBase
    {
        public ImsUserService(ILogger logger) : base(logger)
        {
        }

        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsUser lmsUser, int courseId, object extraData = null)
        {
            return OperationResultWithData<List<LmsUserDTO>>.Success(new List<LmsUserDTO>());
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string userId, int courseId, out string error, object param = null)
        {
            error = null;
            return new List<LmsUserDTO>();
        }
    }
}