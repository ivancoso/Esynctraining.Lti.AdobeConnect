namespace EdugameCloud.Lti.API.Canvas
{
    using System;
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using RestSharp;

    public class CourseAPI
    {
        private static string[] canvasRoles = new[] { "Teacher", "Ta", "Student", "Observer", "Designer" };

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

            ret.AddRange(response.Data);

            return ret;
        }

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