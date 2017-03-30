using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API
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


        protected ILogger logger { get; private set; }


        protected LmsUserServiceBase(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // <param name="currentUser">When we get all users for course, we use admin's token (currentUser.token)</param>
        // <param name="lmsUserId">User Id we want to retrieve information for from LMS. Can be different from currentUser</param>
        public virtual LmsUserDTO GetUser(ILmsLicense lmsCompany, string lmsUserId, int courseId, out string error, LtiParamDTO extraData = null)
        {
            // meeting parameter(second) is used for Blackboard calls of the below method.
            // BB has its own implementation of GetUser, so null can be passed here until we use meeting for retrieving user
            return GetUsersOldStyle(lmsCompany, courseId, out error, extraData)
                .FirstOrDefault(u => u.Id == lmsUserId);
        }

        public virtual bool CanRetrieveUsersFromApiForCompany(ILmsLicense company)
        {
            return company.AdminUser != null;
        }

        public abstract OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany, int courseId, LtiParamDTO extraData = null);

        public abstract List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, out string error, LtiParamDTO param = null);


        // TODO: ROLEMAPPING
        protected List<LmsUserDTO> GroupUsers(List<LmsUserDTO> users)
        {
            if (users != null && users.Any())
            {
                users = users.GroupBy(u => u.Id).Select(
                    ug =>
                    {
                        foreach (string orderRole in order)
                        {
                            string role = orderRole;
                            LmsUserDTO userDTO =
                                ug.FirstOrDefault(u => role.Equals(u.LmsRole, StringComparison.OrdinalIgnoreCase));
                            if (userDTO != null)
                            {
                                return userDTO;
                            }
                        }

                        return ug.First();
                    }).ToList();

                return users;
            }

            return new List<LmsUserDTO>();
        }

    }

}