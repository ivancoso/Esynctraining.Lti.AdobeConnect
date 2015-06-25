using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI canvasApi;
        private readonly LmsUserModel lmsUserModel;

        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger, LmsUserModel lmsUserModel) : base(logger)
        {
            this.canvasApi = canvasApi;
            this.lmsUserModel = lmsUserModel;
        }

        public override LmsUserDTO GetUser(LmsCompany lmsCompany, LmsUser currentUser, LmsCourseMeeting meeting, 
            string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            var token = currentUser.Return(
                u => u.Token,
                string.Empty);

            // probably it makes sence to call this method with company admin's token - for getting emails within one call
            var user = canvasApi.GetCourseUser(
                lmsUserId,
                lmsCompany.LmsDomain,
                token,
                courseId);
            
            error = null;
            return user;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            // todo: check for all arguments == null
            if (lmsUser.Id == 0 && lmsCompany.AdminUser == null)
            {
                var message =
                    string.Format("There is no admin user set for LmsCompanyId={0}. MeetingId={1}, CourseId={2}",
                    lmsCompany.Id, meeting != null ? meeting.Id : (object)string.Empty, courseId);
                logger.Error(message);
                return OperationResult<List<LmsUserDTO>>.Error(message);
            }

            List<LmsUserDTO> users = FetchUsers(lmsCompany, lmsUser, courseId);

            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, string userId, int courseId, out string error, bool forceUpdate = false, object param = null)
        //        public List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string canvasUserId, int courseId)
        {
            LmsUser lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(userId, lmsCompany.Id).Value;
            string token = lmsUser.Return(u => u.Token, string.Empty);

            List<LmsUserDTO> users = FetchUsers(lmsCompany, lmsUser, courseId);

            error = null;
            return GroupUsers(users);
        }


        private List<LmsUserDTO> FetchUsers(LmsCompany lmsCompany, LmsUser lmsUser, int courseId)
        {
            string token = ((lmsCompany.AdminUser != null) && (lmsCompany.AdminUser.Token != null))
                ? lmsCompany.AdminUser.Token
                : token = lmsUser.Token;

            List<LmsUserDTO> users = canvasApi.GetUsersForCourse(
                lmsCompany.LmsDomain,
                token,
                courseId);

            List<string> courseTeacherTokens = null;
            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.primary_email)))
            {
                IEnumerable<string> courseTeachers = users
                    .Where(u => u.lms_role.ToUpper().Equals("TEACHER") || u.lms_role.ToUpper().Equals("TA"))
                    .Select(u => u.id)
                    .Distinct();

                courseTeacherTokens =
                    lmsUserModel.GetByUserIdAndCompanyLms(courseTeachers.ToArray(), lmsCompany.Id)
                    .Where(t => !string.IsNullOrWhiteSpace(t.Token))
                    .Select(v => v.Token)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(token))
                    users = canvasApi.GetUsersForCourse(
                        lmsCompany.LmsDomain,
                        courseTeacherTokens.FirstOrDefault(),
                        courseId);
            }

            // TRICK: leave this for extra save executing.
            // emails are now included in the above api call
            // leaving code below for old-style support for getting user email (for example for students)
            if (users.Any(x => string.IsNullOrEmpty(x.primary_email)))
            {
                logger.InfoFormat("[Canvas GetUsers] API did not return emails. CourseID={0}. LMSCompanyID:{1}. Current LmsUser ID: {2}.", courseId, lmsCompany.Id, lmsUser.Id);

                //if (!adminCourseTokens.Contains(token))
                //{
                //    adminCourseTokens.Add(token);
                //}
                //if (lmsCompany.AdminUser != null && lmsCompany.AdminUser.Token != null
                //    && !adminCourseTokens.Contains(lmsCompany.AdminUser.Token))
                //{
                //    adminCourseTokens.Add(lmsCompany.AdminUser.Token);
                //}

                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.primary_email))
                    {
                        // todo: investigate cases(except when role=student) when API does not return emails and probably remove this code
                        logger.InfoFormat("[Canvas GetUsers] Api did not return email for user with id={0}", user.id);
                        foreach (var adminToken in courseTeacherTokens)
                        {
                            if (!string.IsNullOrEmpty(user.primary_email))
                            {
                                break;
                            }
                            canvasApi.AddMoreDetailsForUser(lmsCompany.LmsDomain, adminToken, user);
                        }
                    }
                }
            }
            return users;
        }

    }

}