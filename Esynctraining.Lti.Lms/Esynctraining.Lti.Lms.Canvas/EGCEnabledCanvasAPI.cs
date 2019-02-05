using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Lms.Canvas.Convertors;
using Esynctraining.Lti.Lms.Canvas.DTOs;
using Esynctraining.Lti.Lms.Canvas.Resources;
using Esynctraining.Lti.Lms.Common.Dto.Canvas;

namespace Esynctraining.Lti.Lms.Canvas
{
    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI, IEGCEnabledCanvasAPI
    {
        private const string StandartTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

        public EGCEnabledCanvasAPI(ILogger logger, IJsonDeserializer jsonDeserializer)
            : base(logger, jsonDeserializer)
        {
        }

        // users
        public async Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string courseId)
        {
            var userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            try
            {
                Validate((string)licenseSettings[LmsLicenseSettingNames.LmsDomain], userToken);

                if (!licenseSettings.ContainsKey(LmsUserSettingNames.Token))
                {
                    _logger.Error($"There is no user token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                    throw new WarningMessageException(Messages.NoLicenseAdmin);
                }

                var token = (string)licenseSettings[LmsUserSettingNames.Token];

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.Error($"Empty token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                    throw new WarningMessageException(Messages.NoLicenseAdmin);
                }

                var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                    ? new RefreshTokenParamsDto
                    {
                        OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                        OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                        RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                    }
                    : null;
                LmsUserDTO user = await FetchCourseUser(userId, 
                                                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain], 
                                                        token, 
                                                        courseId,
                                                        refreshTokenParams);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error($"[EGCEnabledCanvasAPI.GetCourseUser] API:{licenseSettings["LmsDomain"]}. UserToken:{userToken}. CourseId:{courseId}. UserId:{userId}.", ex);
                throw;
            }
        }

        private async Task<LmsUserDTO> FetchCourseUser(string userId, string domain, string userToken, string courseId, RefreshTokenParamsDto refreshTokenParams)
        {
            try
            {
                Validate(domain, userToken);

                var client = CreateRestClient(domain);

                var link = string.Format("/api/v1/courses/{0}/users/{1}?include[]=email&include[]=enrollments",
                        courseId, userId);

                RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, refreshTokenParams);

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
        
        public async Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string courseId, Dictionary<string, object> licenseSettings)
        {
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            try
            {
                Validate(domain, userToken);

                var link = string.Format("/api/v1/courses/{0}/users?per_page={1}&include[]=email&include[]=enrollments",
                    courseId, 100); // default is 10 records per page, max - 100
                var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                    ? new RefreshTokenParamsDto
                    {
                        OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                        OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                        RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                    }
                    : null;
                var users = await GetAllPages<CanvasLmsUserDTO>(domain, link, userToken, refreshTokenParams,
                    x => x.enrollments.Any(enr =>
                        enr.course_id == courseId && enr.enrollment_state == CanvasEnrollment.EnrollmentState.Active));
                return users.Select(user => new LmsUserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Login = user.login_id,
                        Name = user.Name,
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

        private async Task<List<T>> GetAllPages<T>(string domain, string link, string userToken, RefreshTokenParamsDto refreshTokenParams, Func<T, bool> filter = null)
        {
            var result = new List<T>();
            var client = CreateRestClient(domain);

            while (!string.IsNullOrEmpty(link))
            {
                RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, refreshTokenParams);

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

        //sections

        public async Task<List<LmsCourseSectionDTO>> GetCourseSections(string domain, string userToken, int courseId)
        {
            try
            {
                Validate(domain, userToken);

                var link = $"/api/v1/courses/{courseId}/sections?per_page=100&include[]=students&include[]=enrollments";
                var sections = await GetAllPages<CanvasCourseSectionDTO>(domain, link, userToken, null);

                return sections.Select(x => new LmsCourseSectionDTO
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Users = x.Students?.Select(s => new LmsUserDTO { Id = s.Id.ToString(), Name = s.Name.ToString() }).ToList()
                            ?? new List<LmsUserDTO>()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetCourseSections] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

        //quizzes

        public async Task<OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey)
        {
            var quizzesResult = await GetItemsForUserAsync(licenseSettings, isSurvey, null);
            if (!quizzesResult.IsSuccess)
                return OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>.Error(quizzesResult.Message);

            var result = quizzesResult.Data.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });

            return result.ToSuccessResult();
        }

        public async Task<OperationResultWithData<IEnumerable<LmsQuizDTO>>> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds)
        {
            try
            {
                var userToken = (string)licenseSettings[LmsUserSettingNames.Token];

                Validate((string)licenseSettings[LmsLicenseSettingNames.LmsDomain], userToken);

                var course = await GetCourse(
                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                        (string)licenseSettings[LmsUserSettingNames.Token],
                        (string)licenseSettings[LmsUserSettingNames.CourseId]);

                var client = CreateRestClient((string)licenseSettings[LmsLicenseSettingNames.LmsDomain]);

                RestRequest request = await CreateRequest(
                    (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                    $"/api/v1/courses/{(string) licenseSettings[LmsUserSettingNames.CourseId]}/quizzes",
                    Method.GET,
                    (string)licenseSettings[LmsUserSettingNames.Token]);
                request.AddParameter("per_page", 1000);

                IRestResponse<List<CanvasQuizDTO>> response = await client.ExecuteTaskAsync<List<CanvasQuizDTO>>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                        (string)licenseSettings[LmsUserSettingNames.Token],
                        (string)licenseSettings[LmsUserSettingNames.CourseId], 
                        BuildInformation(response));
                    throw new InvalidOperationException(
                        $"[EGCEnabledCanvasAPI.GetItemsForUser] Canvas returns '{response.StatusDescription}'");
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
                            (await GetQuestionsForQuiz(
                                (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                                (string)licenseSettings[LmsUserSettingNames.Token],
                                (string)licenseSettings[LmsUserSettingNames.CourseId],
                                q.id)).ToArray();

                        CanvasQuizParser.Parse(q);

                        foreach (var question in q.questions)
                        {
                            foreach (var fileIndex in question.files.Keys)
                            {
                                var file = question.files[fileIndex];
                                if (!file.fileUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var canvasFile = await GetFile(
                                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                                        (string)licenseSettings[LmsUserSettingNames.Token],
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

                return ((IEnumerable<LmsQuizDTO>) response.Data).ToSuccessResult(); //todo: handle error case
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetItemsForUser] API:{0}. UserToken:{1}. CourseId:{2}.",
                    (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                    (string)licenseSettings[LmsUserSettingNames.Token],
                    (string)licenseSettings[LmsUserSettingNames.CourseId]);
                throw;
            }
        }

        public async Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers)
        {
            throw new NotImplementedException();
        }

        public async Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId)
        {
            var userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            var api = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];

            IRestResponse<CanvasQuizDto> response;

            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/courses/{courseId}/quizzes/{quizId}",
                    Method.PUT,
                    userToken);

                request.AddParameter("quiz[published]", true);

                response = await client.ExecuteTaskAsync<CanvasQuizDto>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.PublishQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}", api, userToken, courseId, quizId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.PublishQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}. {4}",
                    api, userToken, courseId, quizId, BuildInformation(response));

                throw new InvalidOperationException(
                    $"[CanvasAPI.PublishQuiz] Canvas returns '{response.StatusDescription}'");
            }
        }

        private async Task<LmsCourseDTO> GetCourse(string api, string userToken, string courseId)
        {
            IRestResponse<LmsCourseDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/courses/{courseId}",
                    Method.GET,
                    userToken);

                response = await client.ExecuteGetTaskAsync<LmsCourseDTO>(request);
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
                throw new InvalidOperationException(
                    $"[CanvasAPI.GetCourse] Canvas returns '{response.StatusDescription}'");
            }
            return response.Data;
        }

        private async Task<CanvasFileDTO> GetFile(string api, string userToken, string fileId)
        {
            IRestResponse<CanvasFileDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/files/{fileId}",
                    Method.GET,
                    userToken);

                response = await client.ExecuteGetTaskAsync<CanvasFileDTO>(request);
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
                throw new InvalidOperationException(
                    $"[CanvasAPI.GetFile] Canvas returns '{response.StatusDescription}'");
            }
            return response.Data;
        }

        private async Task<List<CanvasQuestionDTO>> GetQuestionsForQuiz(string api, string userToken, string courseId, int quizId)
        {
            IRestResponse<List<CanvasQuestionDTO>> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/courses/{courseId}/quizzes/{quizId}/questions",
                    Method.GET,
                    userToken);
                request.AddParameter("per_page", 1000);
                response = await client.ExecuteGetTaskAsync<List<CanvasQuestionDTO>>(request);
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
                throw new InvalidOperationException(
                    $"[CanvasAPI.GetQuestionsForQuiz] Canvas returns '{response.StatusDescription}'");
            }

            return response.Data;
        }

        //quizzes - canvas only
        public async Task<CanvasQuizSubmissionDTO> CreateQuizSubmission(string api, string userToken, int courseId, int quizId)
        {
            IRestResponse<CanvasQuizSubmissionResultDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/courses/{courseId}/quizzes/{quizId}/submissions",
                    Method.POST,
                    userToken);

                response = await client.ExecuteTaskAsync<CanvasQuizSubmissionResultDTO>(request);
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
                throw new InvalidOperationException(
                    $"[CanvasAPI.GetSubmissionForQuiz] Canvas returns '{response.StatusDescription + ":" + (response.ErrorMessage ?? "")}'");
            }
            return response.Data.quiz_submissions.Single();
        }

        public async Task AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission)
        {
            IRestResponse response = null;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/quiz_submissions/{submission.id}/questions",
                    Method.POST,
                    userToken);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(submission);

                response = await client.ExecuteTaskAsync(request);
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
                throw new InvalidOperationException(
                    $"[CanvasAPI.AnswerQuestionsForQuiz] Canvas returns '{response.StatusDescription}'");
            }
        }

        public async Task CompleteQuizSubmission(string api, string userToken, int courseId, CanvasQuizSubmissionDTO submission)
        {
            IRestResponse response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    $"/api/v1/courses/{courseId}/quizzes/{submission.quiz_id}/submissions/{submission.id}/complete",
                    Method.POST,
                    userToken);
                request.AddParameter("attempt", submission.attempt);
                request.AddParameter("validation_token", submission.validation_token);

                response = await client.ExecuteTaskAsync(request);
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
                throw new InvalidOperationException(
                    $"[EGCEnabledCanvasAPI.ReturnSubmissionForQuiz] Canvas returns '{response.StatusDescription}'");
            }
        }

        //calendar

        public async Task<LmsCalendarEventDTO> CreateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            var client = CreateRestClient(domain);

            string startTimeUtc = lmsEvent.StartAt.ToString(StandartTimeFormat);
            string endTimeUtc = lmsEvent.EndAt.ToString(StandartTimeFormat);
            string eventTitle = HttpUtility.UrlEncode(lmsEvent.Title);
            var link = $"/api/v1/calendar_events?calendar_event[context_code]=course_{courseId}&calendar_event[title]={eventTitle}&calendar_event[start_at]={startTimeUtc}&calendar_event[end_at]={endTimeUtc}";

            var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                ? new RefreshTokenParamsDto
                {
                    OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                    OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                    RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                }
                : null;
            RestRequest request = await CreateRequest(domain, link, Method.POST, userToken, refreshTokenParams);
            IRestResponse<CanvasCalendarEventDTO> response = await client.ExecuteTaskAsync<CanvasCalendarEventDTO>(request);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.Error($"[Canvas API error CreateCalendarEvent] Response error: {response.Content}");
                return null;
            }

            LmsCalendarEventDTO lmsCalendarEvent = CanvasCalendarConvertor.ConvertToLmsCalendarEvent(response.Data);
            return lmsCalendarEvent;
        }

        public async Task<LmsCalendarEventDTO> UpdateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            var client = CreateRestClient(domain);

            string startTimeUtc = lmsEvent.StartAt.ToString(StandartTimeFormat);
            string endTimeUtc = lmsEvent.EndAt.ToString(StandartTimeFormat);
            string eventTitle = HttpUtility.UrlEncode(lmsEvent.Title);
            var link = $"/api/v1/calendar_events/{lmsEvent.Id}?calendar_event[title]={eventTitle}&calendar_event[start_at]={startTimeUtc}&calendar_event[end_at]={endTimeUtc}";

            var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                ? new RefreshTokenParamsDto
                {
                    OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                    OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                    RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                }
                : null;
            RestRequest request = await CreateRequest(domain, link, Method.PUT, userToken, refreshTokenParams);
            IRestResponse<CanvasCalendarEventDTO> response = await client.ExecuteTaskAsync<CanvasCalendarEventDTO>(request);

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
            }
            LmsCalendarEventDTO lmsCalendarEvent = CanvasCalendarConvertor.ConvertToLmsCalendarEvent(response.Data);
            return lmsCalendarEvent;

        }

        public async Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string userId, Dictionary<string, object> licenseSettings)
        {

            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            var client = CreateRestClient(domain);
            var link = $"/api/v1/users/{userId}/calendar_events";
            var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                ? new RefreshTokenParamsDto
                {
                    OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                    OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                    RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                }
                : null;
            RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, refreshTokenParams);
            IRestResponse<IEnumerable<CanvasCalendarEventDTO>> response = await client.ExecuteTaskAsync<IEnumerable<CanvasCalendarEventDTO>>(request);
            
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
            }

            IEnumerable<CanvasCalendarEventDTO> events = response.Data;
            return events.Select(e => CanvasCalendarConvertor.ConvertToLmsCalendarEvent(e));
        }

        public async Task DeleteCalendarEvents(int eventId, Dictionary<string, object> licenseSettings)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];

            var client = CreateRestClient(domain);
            var link = $"/api/v1/calendar_events/{eventId}";
            var refreshTokenParams = licenseSettings.ContainsKey(LmsUserSettingNames.RefreshToken)
                ? new RefreshTokenParamsDto
                {
                    OAuthId = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                    OAuthKey = (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                    RefreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken]
                }
                : null;
            RestRequest request = await CreateRequest(domain, link, Method.DELETE, userToken, refreshTokenParams);
            IRestResponse<IEnumerable<CanvasCalendarEventDTO>> response = await client.ExecuteTaskAsync<IEnumerable<CanvasCalendarEventDTO>>(request);

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
            }
        }
    }

}
