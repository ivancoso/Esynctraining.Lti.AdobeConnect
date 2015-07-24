using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace EdugameCloud.Lti.Canvas
{
    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI, IEGCEnabledCanvasAPI
    {
        private LmsUserModel _lmsUserModel;


        private LmsUserModel LmsUserModel
        {
            get 
            {
                return _lmsUserModel ?? (_lmsUserModel = IoC.Resolve<LmsUserModel>());
            }
        }


        public EGCEnabledCanvasAPI(ILogger logger)
            : base(logger)
        {
        }

        //public static LmsQuizDTO GetQuizById(string api, string usertoken, int courseid, string quizid)
        //{
        //    var client = CreateRestClient(api);

        //    RestRequest request = CreateRequest(
        //        api,
        //        string.Format("/api/v1/courses/{0}/quizzes/{1}", courseid, quizid),
        //        Method.GET,
        //        usertoken);

        //    IRestResponse<LmsQuizDTO> response = client.Execute<LmsQuizDTO>(request);

        //    response.Data.questions = GetQuestionsForQuiz(api, usertoken, courseid, response.Data.id).ToArray();

        //    return response.Data;
        //}

        public List<CanvasQuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string userToken,
            int courseId,
            int quizId)
        {
            IRestResponse<CanvasQuizSubmissionResultDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseId, quizId),
                    Method.POST,
                    userToken);

                response = client.Execute<CanvasQuizSubmissionResultDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetSubmissionForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}.", api, userToken, courseId, quizId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[EGCEnabledCanvasAPI.GetSubmissionForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}. {4}",
                    api, userToken, courseId, quizId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetSubmissionForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data.quiz_submissions;
        }

        public LmsUserDTO GetCourseUser(string userId, LmsCompany lmsCompany, string userToken, int courseId)
        {
            try
            {
                Validate(lmsCompany.LmsDomain, userToken);

                string token = ((lmsCompany.AdminUser != null) && (lmsCompany.AdminUser.Token != null))
                    ? lmsCompany.AdminUser.Token
                    : userToken;

                LmsUserDTO user = FetchCourseUser(userId, lmsCompany.LmsDomain, token, courseId);

                List<string> courseTeacherTokens = null;
                // IF emails is NOT included (for student + lmsCompany.AdminUser == null)
                if (string.IsNullOrEmpty(user.primary_email))
                {
                    List<LmsUserDTO> users = GetUsersForCourse(
                        lmsCompany.LmsDomain,
                        token,
                        courseId);

                    IEnumerable<string> courseTeachers = users
                        .Where(u => u.lms_role.ToUpper().Equals("TEACHER") || u.lms_role.ToUpper().Equals("TA"))
                        .Select(u => u.id)
                        .Distinct();

                    courseTeacherTokens =
                        LmsUserModel.GetByUserIdAndCompanyLms(courseTeachers.ToArray(), lmsCompany.Id)
                        .Where(t => !string.IsNullOrWhiteSpace(t.Token))
                        .Select(v => v.Token)
                        .ToList();

                    foreach (string teacherToken in courseTeacherTokens)
                    {
                        user = FetchCourseUser(userId, lmsCompany.LmsDomain, courseTeacherTokens.FirstOrDefault(), courseId);
                        if (!string.IsNullOrEmpty(user.primary_email))
                            break;
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}.", lmsCompany.LmsDomain, userToken, courseId, userId);
                throw;
            }
        }

        private LmsUserDTO FetchCourseUser(string userId, string domain, string userToken, int courseId)
        {
            try
            {
                Validate(domain, userToken);

                var client = CreateRestClient(domain);

                var link = string.Format("/api/v1/courses/{0}/users/{1}?include[]=email&include[]=enrollments",
                        courseId, userId);

                RestRequest request = CreateRequest(domain, link, Method.GET, userToken);

                IRestResponse<CanvasLmsUserDTO> response = client.Execute<CanvasLmsUserDTO>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}. {4}",
                        domain, userToken, courseId, userId, BuildInformation(response));
                    throw new InvalidOperationException(string.Format("[EGCEnabledCanvasAPI.FetchCourseUser] Canvas returns '{0}'", response.StatusDescription));
                }

                var result = response.Data;
                if (result != null)
                {
                    result.primary_email = result.email;
                    SetRole(result, courseId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}.", domain, userToken, courseId, userId);
                throw;
            }
        }

        public List<LmsUserDTO> GetUsersForCourse(string domain, string userToken, int courseId)
        {
            try
            {
                Validate(domain, userToken);

                var result = new List<LmsUserDTO>();
                var client = CreateRestClient(domain);

                //foreach (string role in CanvasAPI.CanvasRoles)
                //{
                //var link = string.Format("/api/v1/courses/{0}/users?enrollment_type={1}&per_page={2}&include[]=email",
                //    courseid,
                //    role,
                //    100); // default is 10 records per page, max - 100

                var link = string.Format("/api/v1/courses/{0}/users?per_page={1}&include[]=email&include[]=enrollments",
                    courseId, 100); // default is 10 records per page, max - 100

                while (!string.IsNullOrEmpty(link))
                {
                    RestRequest request = CreateRequest(domain, link, Method.GET, userToken);

                    IRestResponse<List<CanvasLmsUserDTO>> response = client.Execute<List<CanvasLmsUserDTO>>(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                        if (errorData != null && errorData.errors != null && errorData.errors.Any())
                        {
                            _logger.ErrorFormat("[Canvas API error] StatusCode:{0}, StatusDescription:{1}, link: {2}, domain:{3}.", response.StatusCode, response.StatusDescription, link, domain);
                            foreach (var error in errorData.errors)
                            {
                                _logger.ErrorFormat("[Canvas API error] Response error: {0}", error.message);
                            }
                        }
                        return result;
                    }

                    link = ExtractNextPageLink(response);

                    List<CanvasLmsUserDTO> us = response.Data;
                    if (us == null)
                    {
                        continue;
                    }
                    us.ForEach(
                        u =>
                        {
                            SetRole(u, courseId); //u.lms_role = role;
                            u.primary_email = u.email; // todo: create separate canvas api class and map it to LmsUserDTO
                        });

                    result.AddRange(us);
                }
                //}

                return result;

            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetUsersForCourse] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

        /// <summary>
        /// The return submission for quiz.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="userToken">
        /// The user token.
        /// </param>
        /// <param name="courseId">
        /// The course id.
        /// </param>
        /// <param name="submission">
        /// The submission.
        /// </param>
        public void CompleteQuizSubmission(
            string api,
            string userToken,
            int courseId,
            CanvasQuizSubmissionDTO submission)
        {
            IRestResponse response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions/{2}/complete", courseId, submission.quiz_id, submission.id),
                    Method.POST,
                    userToken);
                request.AddParameter("attempt", submission.attempt);
                request.AddParameter("validation_token", submission.validation_token);

                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.ReturnSubmissionForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. SubmissionQuizId:{3}. SubmissionId:{4}.", 
                    api, userToken, courseId, submission.quiz_id, submission.id);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[EGCEnabledCanvasAPI.ReturnSubmissionForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. SubmissionQuizId:{3}. SubmissionId:{4}. {5}",
                    api, userToken, courseId, submission.quiz_id, submission.id, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[EGCEnabledCanvasAPI.ReturnSubmissionForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
        }

        /// <summary>
        /// The get items for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS user parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is survey.
        /// </param>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{LmsQuizDTO}"/>.
        /// </returns>
        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            try
            {
                Validate(lmsUserParameters.CompanyLms.LmsDomain, lmsUserParameters.LmsUser.Token);

                var course = GetCourse(
                        lmsUserParameters.CompanyLms.LmsDomain,
                        lmsUserParameters.LmsUser.Token,
                        lmsUserParameters.Course);

                var client = CreateRestClient(lmsUserParameters.CompanyLms.LmsDomain);

                RestRequest request = CreateRequest(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    string.Format("/api/v1/courses/{0}/quizzes", lmsUserParameters.Course),
                    Method.GET,
                    lmsUserParameters.LmsUser.Token);
                request.AddParameter("per_page", 1000);

                IRestResponse<List<CanvasQuizDTO>> response = client.Execute<List<CanvasQuizDTO>>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                        lmsUserParameters.CompanyLms.LmsDomain, lmsUserParameters.LmsUser.Token, lmsUserParameters.Course, BuildInformation(response));
                    throw new InvalidOperationException(string.Format("[EGCEnabledCanvasAPI.GetItemsForUser] Canvas returns '{0}'", response.StatusDescription));
                }

                if (quizIds != null)
                {
                    response.Data = response.Data.Where(q => quizIds.Contains(q.id)).ToList();
                }

                response.Data =
                    response.Data.Where(
                        q =>
                        isSurvey ? q.quiz_type.ToLower().Contains("survey") : (!q.quiz_type.ToLower().Contains("survey")))
                        .ToList();

                foreach (CanvasQuizDTO q in response.Data)
                {
                    if (quizIds != null)
                    {
                        q.questions =
                            GetQuestionsForQuiz(
                                lmsUserParameters.CompanyLms.LmsDomain,
                                lmsUserParameters.LmsUser.Token,
                                lmsUserParameters.Course,
                                q.id).ToArray();

                        CanvasQuizParser.Parse(q);

                        foreach (var question in q.questions)
                        {
                            foreach (var fileIndex in question.files.Keys)
                            {
                                var file = question.files[fileIndex];
                                if (!file.fileUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var canvasFile = GetFile(
                                        lmsUserParameters.CompanyLms.LmsDomain,
                                        lmsUserParameters.LmsUser.Token,
                                        CanvasQuizParser.GetFileId(file.fileUrl));

                                    file.fileUrl = canvasFile.url;
                                }

                                question.question_text = CanvasQuizParser.ReplaceFilePlaceHolder(question.question_text, fileIndex, file);
                                question.answers.ForEach(
                                    a => a.text = CanvasQuizParser.ReplaceFilePlaceHolder(a.text, fileIndex, file));
                            }
                        }
                    }
                    q.course = course.id;
                    q.courseName = course.name;
                }

                error = string.Empty;
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}.",
                    lmsUserParameters.CompanyLms.LmsDomain, lmsUserParameters.LmsUser.Token, lmsUserParameters.Course);
                throw;
            }
        }

        /// <summary>
        /// The send answers.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS user parameters.
        /// </param>
        /// <param name="json">
        /// The JSON.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not yet implemented
        /// </exception>
        public void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get items info for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The LMS user parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is survey.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{LmsQuizInfoDTO}"/>.
        /// </returns>
        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizzes = this.GetItemsForUser(lmsUserParameters, isSurvey, null, out error);
            return quizzes.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });
        }


        private static string ExtractNextPageLink(IRestResponse<List<CanvasLmsUserDTO>> response)
        {
            var link = string.Empty;

            foreach (var h in response.Headers)
            {
                if (h.Name.Equals("Link", StringComparison.InvariantCultureIgnoreCase))
                {
                    link = h.Value.ToString();
                    var index = link.IndexOf("rel=\"next\"");
                    if (index > -1)
                    {
                        link = link.Substring(0, index - 2);
                        index = link.LastIndexOf(">");
                        if (index > -1)
                        {
                            link = link.Substring(0, index);
                            index = link.LastIndexOf("<");
                            if (index > -1)
                            {
                                link = link.Substring(index + 1);
                                index = link.IndexOf("/api/v1/");
                                if (index > -1)
                                {
                                    link = link.Substring(index);
                                }
                                break;
                            }
                        }
                    }
                }
                link = string.Empty;
            }
            return link;
        }

        private void SetRole(CanvasLmsUserDTO userDto, int courseid)
        {
            if (userDto.enrollments != null)
            {
                var enrollment = userDto.enrollments.FirstOrDefault(x => x.course_id == courseid);
                if (enrollment != null)
                {
                    userDto.lms_role = enrollment.role.Replace("Enrollment", String.Empty);
                    return;
                }
            }

            _logger.WarnFormat("[Canvas API] User without role. CourseId:{0}, UserId:{1}",
                        courseid, userDto.id);
        }

    }

}
