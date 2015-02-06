namespace EdugameCloud.Lti.API.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using RestSharp;

    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI
    {
        /// <summary>
        /// The get quiz by id.
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
        /// The <see cref="LmsQuizDTO"/>.
        /// </returns>
        public static LmsQuizDTO GetQuizById(string api, string usertoken, int courseid, string quizid)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}", courseid, quizid),
                Method.GET,
                usertoken);

            IRestResponse<LmsQuizDTO> response = client.Execute<LmsQuizDTO>(request);

            response.Data.questions = GetQuestionsForQuiz(api, usertoken, courseid, response.Data.id).ToArray();

            return response.Data;
        }

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
        public static List<QuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            int quizid)
        {
            var ret = new List<QuizSubmissionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseid, quizid),
                Method.POST,
                usertoken);

            IRestResponse<QuizSubmissionResultDTO> response = client.Execute<QuizSubmissionResultDTO>(request);

            ret.AddRange(response.Data.quiz_submissions);

            return ret;
        }

        /// <summary>
        /// The get users for course.
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
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public static List<LmsUserDTO> GetUsersForCourse(string api, string usertoken, int courseid)
        {
            var ret = new List<LmsUserDTO>();
            var client = CreateRestClient(api);

            // ReSharper disable once RedundantNameQualifier
            foreach (string role in CanvasAPI.CanvasRoles)
            {
                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/users?enrollment_type={1}&per_page={2}", courseid, role, 100),
                    Method.GET,
                    usertoken);

                IRestResponse<List<LmsUserDTO>> response = client.Execute<List<LmsUserDTO>>(request);

                List<LmsUserDTO> us = response.Data;
                us.ForEach(
                    u =>
                    {
                        u.lms_role = role;
                    });

                ret.AddRange(us);
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
        public static void ReturnSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            QuizSubmissionDTO submission)
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
            IRestResponse<List<LmsQuizDTO>> response = client.Execute<List<LmsQuizDTO>>(request);

            if (quizIds != null)
            {
                response.Data = response.Data.Where(q => quizIds.Contains(q.id)).ToList();
            }

            response.Data =
                response.Data.Where(
                    q =>
                    isSurvey ? q.quiz_type.ToLower().Contains("survey") : (!q.quiz_type.ToLower().Contains("survey")))
                    .ToList();

            
            foreach (LmsQuizDTO q in response.Data)
            {
                if (quizIds != null)
                {
                    q.questions =
                        GetQuestionsForQuiz(
                            lmsUserParameters.CompanyLms.LmsDomain,
                            lmsUserParameters.LmsUser.Token,
                            lmsUserParameters.Course,
                            q.id).ToArray();
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
        public void SendAnswers(LmsUserParameters lmsUserParameters, string json, bool isSurvey)
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
