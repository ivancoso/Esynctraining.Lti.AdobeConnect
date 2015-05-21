namespace EdugameCloud.Lti.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using RestSharp;

    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI, IEGCEnabledCanvasAPI
    {
        ///// <summary>
        ///// The get quiz by id.
        ///// </summary>
        ///// <param name="api">
        ///// The API.
        ///// </param>
        ///// <param name="usertoken">
        ///// The user token.
        ///// </param>
        ///// <param name="courseid">
        ///// The course id.
        ///// </param>
        ///// <param name="quizid">
        ///// The quiz id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="LmsQuizDTO"/>.
        ///// </returns>
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

        /// <summary>
        /// The get submission for quiz.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="quizid">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="List{QuizSubmissionDTO}"/>.
        /// </returns>
        public List<CanvasQuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            int quizid)
        {
            var ret = new List<CanvasQuizSubmissionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseid, quizid),
                Method.POST,
                usertoken);

            IRestResponse<CanvasQuizSubmissionResultDTO> response = client.Execute<CanvasQuizSubmissionResultDTO>(request);

            ret.AddRange(response.Data.quiz_submissions);

            return ret;
        }

        public LmsUserDTO GetCourseUser(string userId, string domain, string usertoken, int courseid)
        {
            var client = CreateRestClient(domain);

            var link = string.Format(
                // we can use many include parameters here: &include[]=email&include[]=enrollments.
                // todo: investigate whether we can retrieve role information from enrollments param, it contains many fields. Then we'd not make api call for each role
                    "/api/v1/courses/{0}/users/{1}?include[]=email&include[]=enrollments",
                    courseid,
                    userId);

            RestRequest request = CreateRequest(domain, link, Method.GET, usertoken);

            IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

            var result = response.Data;
            if (result != null)
            {
                result.primary_email = result.email;
//                result.lms_role = 
            }

            return result;

        }

        public List<LmsUserDTO> GetUsersForCourse(string domain, string usertoken, int courseid)
        {
            var ret = new List<LmsUserDTO>();
            var client = CreateRestClient(domain);

            // ReSharper disable once RedundantNameQualifier
            foreach (string role in CanvasAPI.CanvasRoles)
            {
                var link = string.Format(
                    // we can use many include parameters here: &include[]=email&include[]=enrollments.
                    // todo: investigate whether we can retrieve role information from enrollments param, it contains many fields. Then we'd not make api call for each role
                    "/api/v1/courses/{0}/users?enrollment_type={1}&per_page={2}&include[]=email",
                    courseid,
                    role,
                    1000);

                while (!string.IsNullOrEmpty(link))
                {
                    RestRequest request = CreateRequest(domain, link, Method.GET, usertoken);

                    IRestResponse<List<LmsUserDTO>> response = client.Execute<List<LmsUserDTO>>(request);

                    link = string.Empty;
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

                    List<LmsUserDTO> us = response.Data;
                    if (us == null)
                    {
                        continue;
                    }
                    us.ForEach(
                        u =>
                            {
                                u.lms_role = role;
                                u.primary_email = u.email; // todo: create separate canvas api class and map it to LmsUserDTO
                            });

                    ret.AddRange(us);
                }
            }

            return ret;
        }

        /// <summary>
        /// The return submission for quiz.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="submission">
        /// The submission.
        /// </param>
        public void ReturnSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            CanvasQuizSubmissionDTO submission)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions/{2}/complete", courseid, submission.quiz_id, submission.id),
                Method.POST,
                usertoken);
            request.AddParameter("attempt", submission.attempt);
            request.AddParameter("validation_token", submission.validation_token);

            client.Execute(request);
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
            var course = GetCourse(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course);

            var ret = new List<LmsQuizDTO>();
            var client = CreateRestClient(lmsUserParameters.CompanyLms.LmsDomain);

            RestRequest request = CreateRequest(
                lmsUserParameters.CompanyLms.LmsDomain,
                string.Format("/api/v1/courses/{0}/quizzes", lmsUserParameters.Course),
                Method.GET,
                lmsUserParameters.LmsUser.Token);
            request.AddParameter("per_page", 1000);
            IRestResponse<List<CanvasQuizDTO>> response = client.Execute<List<CanvasQuizDTO>>(request);

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
            
            ret.AddRange(response.Data);
            error = string.Empty;

            return ret;
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
    }
}
