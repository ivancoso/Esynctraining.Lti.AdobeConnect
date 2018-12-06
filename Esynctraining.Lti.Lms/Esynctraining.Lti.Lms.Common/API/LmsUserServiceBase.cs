using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public abstract class LmsUserServiceBase
    {
        private static readonly string[] order = new string[]
        {
            "owner",
            "author",
            "course builder",
            "teacher",
            "instructor",
            "teaching assistant",
            "ta",
            "designer",
            "student",
            "learner",
            "reader",
            "guest",
        };


        protected ILogger Logger { get; }


        protected LmsUserServiceBase(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // <param name="lmsUserId">User Id we want to retrieve information for from LMS. Can be different from currentUser</param>
        public virtual async Task<OperationResultWithData<LmsUserDTO>> GetUser(
            Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, LtiParamDTO extraData = null)
        {
            // meeting parameter(second) is used for Blackboard calls of the below method.
            // BB has its own implementation of GetUser, so null can be passed here until we use meeting for retrieving user

            var usersResult = await GetUsers(licenseSettings, courseId, extraData);
            if (usersResult.IsSuccess)
            {
                var user = usersResult.Data.FirstOrDefault(u => u.Id == lmsUserId);
                if (user != null)
                {
                    return new OperationResultWithData<LmsUserDTO>
                    {
                        Data = user,
                        Message = usersResult.Message,
                        IsSuccess = true
                    };
                }
                else
                {
                    return OperationResultWithData<LmsUserDTO>.Error("User not found in course");
                }
            }

            return OperationResultWithData<LmsUserDTO>.Error(usersResult.Message);
        }

        public abstract Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(
            Dictionary<string, object> licenseSettings, string courseId, LtiParamDTO extraData = null);

        // TODO: ROLEMAPPING
        protected List<LmsUserDTO> GroupUsers(List<LmsUserDTO> users)
        {
            if (users == null)
                return new List<LmsUserDTO>();
            if (users.Count == 0)
                return new List<LmsUserDTO>();

            users = users
                .GroupBy(u => u.Id)
                .Select(
                ug =>
                {
                    foreach (string orderRole in order)
                    {
                        LmsUserDTO userDTO =
                            ug.FirstOrDefault(u => orderRole.Equals(u.LmsRole, StringComparison.OrdinalIgnoreCase));
                        if (userDTO != null)
                        {
                            return userDTO;
                        }
                    }

                    return ug.First();
                }).ToList();

            return users;
        }

    }

}