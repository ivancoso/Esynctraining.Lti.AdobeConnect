using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API
{
    public abstract class LmsUserServiceBase
    {
        protected ILogger logger { get; private set; }

        protected LmsUserServiceBase(ILogger logger)
        {
            this.logger = logger;
        }

        /// <param name="currentUser">When we get all users for course, we use admin's token (currentUser.token)</param>
        /// <param name="lmsUserId">User Id we want to retrieve information for from LMS. Can be different from currentUser</param>
        public virtual LmsUserDTO GetUser(LmsCompany lmsCompany, string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            // meeting parameter(second) is used for Blackboard calls of the below method.
            // BB has its own implementation of GetUser, so null can be passed here until we use meeting for retrieving user
            return GetUsersOldStyle(lmsCompany, lmsUserId, courseId, out error, forceUpdate)
                .FirstOrDefault(u => u.id == lmsUserId);
        }

        public virtual bool CanRetrieveUsersFromApiForCompany(LmsCompany company)
        {
            return company.AdminUser != null;
        }

        public abstract OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false);

        public abstract List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string userId, int courseId, out string error, bool forceUpdate = false, object param = null);


        // TODO: ROLEMAPPING
        protected List<LmsUserDTO> GroupUsers(List<LmsUserDTO> users)
        {
            if (users != null && users.Any())
            {
                var order = new List<string>
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
                users = users.GroupBy(u => u.id).Select(
                    ug =>
                    {
                        foreach (string orderRole in order)
                        {
                            string role = orderRole;
                            LmsUserDTO userDTO =
                                ug.FirstOrDefault(u => role.Equals(u.lms_role, StringComparison.OrdinalIgnoreCase));
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