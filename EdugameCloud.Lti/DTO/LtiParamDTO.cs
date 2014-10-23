namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The LTI parameter DTO.
    /// </summary>
    public class LtiParamDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the context id.
        /// </summary>
        public string context_id { get; set; }

        /// <summary>
        /// Gets or sets the context label.
        /// </summary>
        public string context_label { get; set; }

        /// <summary>
        /// Gets or sets the context title.
        /// </summary>
        public string context_title { get; set; }

        /// <summary>
        /// Gets or sets the custom AC folder SCO id.
        /// </summary>
        public string custom_ac_folder_sco_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas API domain.
        /// </summary>
        public string custom_canvas_api_domain { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas course id.
        /// </summary>
        public int custom_canvas_course_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user login id.
        /// </summary>
        public string custom_canvas_user_login_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user id.
        /// </summary>
        public string custom_canvas_user_id { get; set; }

        /// <summary>
        /// Gets or sets the launch presentation return url.
        /// </summary>
        public string launch_presentation_return_url { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        public string layout { get; set; }

        /// <summary>
        /// Gets or sets the LIS person contact email primary.
        /// </summary>
        public string lis_person_contact_email_primary { get; set; }

        /// <summary>
        /// Gets or sets the LIS person name family.
        /// </summary>
        public string lis_person_name_family { get; set; }

        /// <summary>
        /// Gets or sets the LIS person name full.
        /// </summary>
        public string lis_person_name_full { get; set; }

        /// <summary>
        /// Gets or sets the LIS person name given.
        /// </summary>
        public string lis_person_name_given { get; set; }

        /// <summary>
        /// Gets or sets the OAUTH consumer key.
        /// </summary>
        public string oauth_consumer_key { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public string roles { get; set; }

        #endregion
    }
}