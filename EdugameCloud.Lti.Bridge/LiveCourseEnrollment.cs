using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.Bridge
{
    public class LiveCourseEnrollment
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
        public string state { get; set; }
    }

    public class LiveSessionResponse
    {
        public int id { get; set; }

        public DateTime? start_at { get; set; }
        public DateTime? end_at { get; set; }
        public string location { get; set; }
        public int? seats { get; set; }
        public int registered_count { get; set; }
        public int present_count { get; set; }
        public int parts_count { get; set; }
        public int parent_id { get; set; }
        public string notes { get; set; }
        public int live_course_id { get; set; }
        public DateTime? concluded_at { get; set; }
        public string enroll_url { get; set; }
        public bool has_web_conference { get; set; }

    }

    public class LiveSessionRequest
    {
        public DateTime? start_at { get; set; }
        public DateTime? end_at { get; set; }
        public string location { get; set; }
        public int seats { get; set; }
        public string notes { get; set; }
        public string timezone { get; set; }
    }

    public class ListSessionsResponse
    {
        public List<LiveSessionResponse> Sessions { get; set; }
    }

    public class WebConferenceRequest
    {
        public string provider { get; set; }
        public string other_provider { get; set; }
        public string meeting_url { get; set; }
        public string access_code { get; set; }
        public string phone { get; set; }
        public string host_key { get; set; }
        public string password { get; set; }
        public string registration_link { get; set; }
        public bool default_session { get; set; }
        public bool apply_all { get; set; }
    }

    public class WebConferenceResponse
    {
        public int id { get; set; }
        public int live_course_session_id { get; set; }
        public string provider { get; set; }
        public string other_provider { get; set; }
        public string meeting_url { get; set; }
        public string access_code { get; set; }
        public string phone { get; set; }
        public string host_key { get; set; }
        public string password { get; set; }
        public string registration_link { get; set; }
    }

    public class WebConferencesResponse
    {
        public List<WebConferenceResponse> web_conferences { get; set; }
    }
}