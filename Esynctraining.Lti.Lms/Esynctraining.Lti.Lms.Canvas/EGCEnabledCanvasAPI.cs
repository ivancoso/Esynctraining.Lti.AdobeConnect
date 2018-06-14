using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdugameCloud.Lti.Canvas;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.Dto.Canvas;
using Newtonsoft.Json;
using RestSharp;

namespace Esynctraining.Lti.Lms.Canvas
{
    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI, IEGCEnabledCanvasAPI
    {
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

        public void AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission)
        {
            IRestResponse response = null;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id),
                    Method.POST,
                    userToken);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(submission);

                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.AnswerQuestionsForQuiz] API:{0}. UserToken:{1}. SubmissionId:{2}.",
                    api, userToken, submission.id);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.AnswerQuestionsForQuiz] API:{0}. UserToken:{1}. SubmissionId:{2}. {3}",
                    api, userToken, submission.id, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.AnswerQuestionsForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
        }

        public CanvasQuizSubmissionDTO CreateQuizSubmission(string api, string userToken, string courseId, int quizId)
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
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetSubmissionForQuiz] Canvas returns '{0}'", response.StatusDescription + ":" + (response.ErrorMessage ?? "")));
            }
            return response.Data.quiz_submissions.Single();
        }

        public async Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string userToken, string courseId)
        {
            try
            {
                Validate((string)licenseSettings["LmsDomain"], userToken);

                //if (lmsCompany.AdminUser == null)
                //{
                //    _logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}.", lmsCompany.Id);
                //    throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
                //}

                var token = (string)licenseSettings["AdminUser.Token"];
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", licenseSettings["LicenseId"]);
                    throw new WarningMessageException(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
                }

                LmsUserDTO user = await FetchCourseUser(userId, (string)licenseSettings["LmsDomain"], token, courseId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}.", licenseSettings["LmsDomain"], userToken, courseId, userId);
                throw;
            }
        }

        private async Task<LmsUserDTO> FetchCourseUser(string userId, string domain, string userToken, string courseId)
        {
            try
            {
                Validate(domain, userToken);

                var client = CreateRestClient(domain);

                var link = string.Format("/api/v1/courses/{0}/users/{1}?include[]=email&include[]=enrollments",
                        courseId, userId);

                RestRequest request = CreateRequest(domain, link, Method.GET, userToken);

                IRestResponse<CanvasLmsUserDTO> response = await client.ExecuteTaskAsync<CanvasLmsUserDTO>(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // NOTE: user not longer exists in Canvas - return null;
                    return null;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}. {4}",
                        domain, userToken, courseId, userId, BuildInformation(response));
                    throw new InvalidOperationException(string.Format("[EGCEnabledCanvasAPI.FetchCourseUser] Canvas returns '{0}'", response.StatusDescription));
                }

                var user = response.Data;
                if (user == null)
                    return null;
                if (!user.enrollments.Any(x => x.course_id == courseId && x.enrollment_state == CanvasEnrollment.EnrollmentState.Active))
                    return null;

                var result = new LmsUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Login = user.login_id,
                    Name = user.Name,
                    PrimaryEmail = user.Email,
                    LmsRole = SetRole(user, courseId)
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}.", domain, userToken, courseId, userId);
                throw;
            }
        }

        public async Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string userToken, string courseId)
        {
            try
            {
                Validate(domain, userToken);

                //foreach (string role in CanvasAPI.CanvasRoles)
                //{
                //var link = string.Format("/api/v1/courses/{0}/users?enrollment_type={1}&per_page={2}&include[]=email",
                //    courseid,
                //    role,
                //    100); // default is 10 records per page, max - 100

                var link = string.Format("/api/v1/courses/{0}/users?per_page={1}&include[]=email&include[]=enrollments",
                    courseId, 100); // default is 10 records per page, max - 100
                var users = await GetAllPages<CanvasLmsUserDTO>(domain, link, userToken,
                    x => x.enrollments.Any(enr =>
                        enr.course_id == courseId && enr.enrollment_state == CanvasEnrollment.EnrollmentState.Active));
                return users.Select(user => new LmsUserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Login = user.login_id,
                        Name = user.Name,
                        PrimaryEmail = user.Email,
                        LmsRole = SetRole(user, courseId),
                        SectionIds = user.enrollments.Select(x => x.course_section_id.ToString()).ToList()
                    }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetUsersForCourse] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

        public async Task<List<LmsCourseSectionDTO>> GetCourseSections(string domain, string userToken, string courseId)
        {
            try
            {
                Validate(domain, userToken);

                var link = $"/api/v1/courses/{courseId}/sections?per_page=100&include[]=students&include[]=enrollments";
                var sections = await GetAllPages<CanvasCourseSectionDTO>(domain, link, userToken);

                return sections.Select(x => new LmsCourseSectionDTO
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Users = x.Students?.Select(s => new LmsUserDTO{Id = s.Id.ToString(), Name = s.Name.ToString()}).ToList()
                        ?? new List<LmsUserDTO>()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetCourseSections] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

        public void CompleteQuizSubmission(
            string api,
            string userToken,
            string courseId,
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

        
        public async Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(
            LmsUserParametersDto lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds)
        {
            var lmsDomain = (string)lmsUserParameters.LicenseSettings["LmsDomain"];

            try
            {
                Validate((string)lmsUserParameters.LicenseSettings["LmsDomain"], lmsUserParameters.Token);

                var course = GetCourse(lmsDomain,
                        lmsUserParameters.Token,
                        lmsUserParameters.CourseId);

                var client = CreateRestClient(lmsDomain);

                RestRequest request = CreateRequest(
                    lmsDomain,
                    string.Format("/api/v1/courses/{0}/quizzes", lmsUserParameters.CourseId),
                    Method.GET,
                    lmsUserParameters.Token);
                request.AddParameter("per_page", 1000);

                IRestResponse<List<CanvasQuizDTO>> response = client.Execute<List<CanvasQuizDTO>>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                        lmsDomain, lmsUserParameters.Token, lmsUserParameters.CourseId, BuildInformation(response));
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
                                lmsDomain,
                                lmsUserParameters.Token,
                                lmsUserParameters.CourseId,
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
                                        lmsDomain,
                                        lmsUserParameters.Token,
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

                return (Data: response.Data, Error: string.Empty);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}.",
                    lmsDomain, lmsUserParameters.Token, lmsUserParameters.CourseId);
                throw;
            }
        }

        public Task SendAnswersAsync(LmsUserParametersDto lmsUserParameters, string json, bool isSurvey, string[] answers)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParametersDto lmsUserParameters, bool isSurvey)
        {
            var quizzes = await GetItemsForUserAsync(lmsUserParameters, isSurvey, null);
            var result = quizzes.Data.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });

            return (Data: result, Error: quizzes.Error);
        }

        public void PublishQuiz(LmsUserParametersDto lmsUserParameters, int courseId, int quizId)
        {
            var domain = (string)lmsUserParameters.LicenseSettings["LmsDomain"];
            var userToken = lmsUserParameters.Token;

            IRestResponse<CanvasQuiz> response;

            try
            {
                Validate(domain, userToken);

                var client = CreateRestClient(domain);

                RestRequest request = CreateRequest(
                    domain,
                    $"/api/v1/courses/{courseId}/quizzes/{quizId}",
                    Method.PUT,
                    userToken);

                request.AddParameter("quiz[published]", true);

                response = client.Execute<CanvasQuiz>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.PublishQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}", domain, userToken, courseId, quizId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.PublishQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}. {4}",
                    domain, userToken, courseId, quizId, BuildInformation(response));

                throw new InvalidOperationException(string.Format("[CanvasAPI.PublishQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
        }

        private async Task<List<T>> GetAllPages<T>(string domain, string link, string userToken, Func<T, bool> filter = null)
        {
            var result = new List<T>();
            var client = CreateRestClient(domain);

            while (!string.IsNullOrEmpty(link))
            {
                RestRequest request = CreateRequest(domain, link, Method.GET, userToken);

                IRestResponse<List<T>> response = await client.ExecuteTaskAsync<List<T>>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                    if (errorData?.errors != null && errorData.errors.Any())
                    {
                        _logger.Error(
                            $"[Canvas API error] StatusCode:{response.StatusCode}, StatusDescription:{response.StatusDescription}, link: {link}, domain:{domain}.");
                        foreach (var error in errorData.errors)
                        {
                            _logger.Error($"[Canvas API error] Response error: {error.message}");
                        }
                    }
                    return result;
                }

                link = ExtractNextPageLink(response);
                IEnumerable<T> records = response.Data;
                if (filter != null)
                {
                    records = records.Where(filter);
                }

                result.AddRange(records);
            }

            return result;
        }

        private static string ExtractNextPageLink<T>(IRestResponse<List<T>> response)
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

        private string SetRole(CanvasLmsUserDTO canvasDto, string courseid)
        {
            if (canvasDto.enrollments != null)
            {
                var enrollment = canvasDto.enrollments.FirstOrDefault(x => x.course_id == courseid);
                if (enrollment != null)
                {
                    return enrollment.role.Replace("Enrollment", String.Empty);
                }
            }

            _logger.WarnFormat("[Canvas API] User without role. CourseId:{0}, UserId:{1}",
                        courseid, canvasDto.Id);
            return null;
        }

        private LmsCourseDTO GetCourse(string api, string userToken, string courseId)
        {
            IRestResponse<LmsCourseDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}", courseId),
                    Method.GET,
                    userToken);

                response = client.Execute<LmsCourseDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetCourse] API:{0}. UserToken:{1}. CourseId:{2}.", api, userToken, courseId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetCourse] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                    api, userToken, courseId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetCourse] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
        }

        private CanvasFileDTO GetFile(string api, string userToken, string fileId)
        {
            IRestResponse<CanvasFileDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/files/{0}", fileId),
                    Method.GET,
                    userToken);

                response = client.Execute<CanvasFileDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetFile] API:{0}. UserToken:{1}. FileId:{2}.", api, userToken, fileId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetFile] API:{0}. UserToken:{1}. FileId:{2}. {3}",
                    api, userToken, fileId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetFile] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
        }

        private List<CanvasQuestionDTO> GetQuestionsForQuiz(string api, string userToken, string courseId, int quizId)
        {
            IRestResponse<List<CanvasQuestionDTO>> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseId, quizId),
                    Method.GET,
                    userToken);
                request.AddParameter("per_page", 1000);
                response = client.Execute<List<CanvasQuestionDTO>>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetQuestionsForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}.", api, userToken, courseId, quizId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetQuestionsForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}. {4}",
                    api, userToken, courseId, quizId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetQuestionsForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
        }

        

        // TODO: move
        private class CanvasQuiz
        {
            public int Id { get; set; }

            public bool Published { get; set; }
        }
    }

}
