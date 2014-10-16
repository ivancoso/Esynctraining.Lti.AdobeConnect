namespace EdugameCloud.Lti.API.Canvas
{
    using System;
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using RestSharp;

    using QuizDTO = EdugameCloud.Lti.DTO.QuizDTO;
    using UserDTO = EdugameCloud.Lti.DTO.UserDTO;

    public class CourseAPI
    {
        private static string[] canvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

        public static List<UserDTO> GetUsersForCourse(string api, string usertoken, int courseid)
        {
            var ret = new List<UserDTO>();
            var client = new RestClient(String.Format("https://{0}", api));
            
            foreach (var role in canvasRoles)
            {
                var request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/users", courseid),
                    Method.GET,
                    usertoken);
                request.AddParameter("enrollment_type", role);

                var response = client.Execute<List<UserDTO>>(request);
                
                var us = response.Data;
                us.ForEach(
                    u =>
                    {
                        u.canvas_role = role;
                        AddMoreDetailsForUser(api, usertoken, u);
                    });

                ret.AddRange(us);
            }

            return ret;
        }

        public static List<QuizDTO> GetQuizzesForCourse(string api, string usertoken, int courseid)
        {

            var ret = new List<QuizDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes", courseid),
                Method.GET,
                usertoken);

            var response = client.Execute<List<QuizDTO>>(request);

            foreach (var q in response.Data)
            {
                q.questions = GetQuestionsForQuiz(api, usertoken, courseid, q.id).ToArray();
            }

            ret.AddRange(response.Data);

            return ret;
        }

        public static List<QuizQuestionDTO> GetQuestionsForQuiz(
            string api,
            string usertoken,
            int courseid,
            int quizid)
        {
            var ret = new List<QuizQuestionDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid),
                Method.GET,
                usertoken);

            var response = client.Execute<List<QuizQuestionDTO>>(request);

            ret.AddRange(response.Data);

            return ret;
        }


        public static List<QuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            int quizid)
        {
            var ret = new List<QuizSubmissionDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseid, quizid),
                Method.POST,
                usertoken);

            var response = client.Execute<QuizSubmissionResultDTO>(request);

            ret.AddRange(response.Data.quiz_submissions);

            return ret;
        }

        public static void ReturnSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            QuizSubmissionDTO submission)
        {
            var ret = new List<QuizSubmissionDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions/{2}/complete", courseid, submission.quiz_id, submission.id),
                Method.POST,
                usertoken);
            request.AddParameter("attempt", submission.attempt);
            request.AddParameter("validation_token", submission.validation_token);

            var response = client.Execute(request);

        }

        public static void AnswerQuestionsForQuiz(
            string api,
            string usertoken,
            QuizSubmissionDTO submission)
        {
            var ret = new List<QuizSubmissionDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id),
                Method.POST,
                usertoken);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(submission);

            var response = client.Execute(request);

        }

        /*
        public static List<AnswerDTO> GetAnswersForQuestion(
            string api,
            string usertoken,
            int courseid,
            int quizid,
            int questionid)
        {
            var ret = new List<QuestionDTO>();
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid),
                Method.GET,
                usertoken);

            var response = client.Execute<List<QuestionDTO>>(request);

            ret.AddRange(response.Data);

            return ret;
        }
        */
        
        public static void AddMoreDetailsForUser(string api, string usertoken, UserDTO user)
        {
            var client = new RestClient(String.Format("https://{0}", api));

            var request = CreateRequest(
                api,
                string.Format("/api/v1/users/{0}/profile", user.id),
                Method.GET,
                usertoken);

            var response = client.Execute<UserDTO>(request);

            if (response.Data != null)
            {
                user.primary_email = response.Data.primary_email;
            }
        }

        public static AnnouncementDTO CreateAnnouncement(string api, string usertoken, int courseid, string title, string message)
        {
            var client = new RestClient(String.Format("https://{0}", api));
            var request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/discussion_topics", courseid),
                    Method.POST,
                    usertoken);
            request.AddParameter("title", title);
            request.AddParameter("message", message);
            request.AddParameter("is_announcement", true);

            var response = client.Execute<AnnouncementDTO>(request);

            return response.Data;
        }

        private static RestRequest CreateRequest(string api, string resource, Method method, string usertoken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + usertoken);
            return request;
        }

    }
}