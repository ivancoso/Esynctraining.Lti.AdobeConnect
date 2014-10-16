namespace EdugameCloud.Lti.DTO
{
    public class LtiParamDTO
    {
        public string oauth_consumer_key { get; set; }

        public int custom_canvas_course_id { get; set; }

        public string custom_ac_folder_sco_id { get; set; }

        public string custom_canvas_api_domain { get; set; }

        public string context_id { get; set; }

        public string context_label { get; set; }

        public string context_title { get; set; }
        
        public string lis_person_name_given { get; set; }

        public string lis_person_name_family { get; set; }

        public string lis_person_name_full { get; set; }

        public string launch_presentation_return_url { get; set; }
        
        public string lis_person_contact_email_primary { get; set; }

        public string custom_canvas_user_login_id { get; set; }

        public string roles { get; set; }

        public string layout { get; set; }
    }
}
