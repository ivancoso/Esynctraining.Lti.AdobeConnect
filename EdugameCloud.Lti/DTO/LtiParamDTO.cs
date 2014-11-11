namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    /// The LTI parameter DTO.
    /// </summary>
    public class LtiParamDTO
    {
        /// <summary>
        /// The referer.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private string refererField;

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
        /// Gets or sets the custom canvas API domain.
        /// </summary>
        public string custom_canvas_api_domain { get; set; }

        /// <summary>
        /// Gets or sets the custom brain honey domain.
        /// </summary>
        public string tool_consumer_instance_guid { get; set; }

        /// <summary>
        /// Gets the LMS domain.
        /// </summary>
        public string lms_domain
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.custom_canvas_api_domain))
                {
                    if (string.IsNullOrWhiteSpace(this.tool_consumer_instance_guid))
                    {
                        return this.LmsDomainFromUrls();
                    }

                    Guid uid;
                    if (Guid.TryParse(this.tool_consumer_instance_guid, out uid))
                    {
                        return this.LmsDomainFromUrls();
                    }

                    return this.tool_consumer_instance_guid;
                }

                return this.custom_canvas_api_domain;
            }
        }

        /// <summary>
        /// Gets or sets the tool consumer info product family code.
        /// </summary>
        public string tool_consumer_info_product_family_code { get; set; }

        /// <summary>
        /// Gets the course id.
        /// </summary>
        public int course_id
        {
            get
            {
                int courseId;
                return this.custom_canvas_course_id == 0 ? int.TryParse(this.context_id, out courseId) ? courseId : this.TryParseBlackBoardCourseId() : this.custom_canvas_course_id;
            }
        }

        /// <summary>
        /// Gets or sets the custom canvas course id.
        /// </summary>
        public int custom_canvas_course_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user login id.
        /// </summary>
        public string custom_canvas_user_login_id { get; set; }

        /// <summary>
        /// Gets the LMS user login id.
        /// </summary>
        public string lms_user_login
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.custom_canvas_user_login_id) ? this.lis_person_sourcedid : this.custom_canvas_user_login_id;
            }
        }

        /// <summary>
        /// Gets or sets the custom canvas user id.
        /// </summary>
        public string custom_canvas_user_id { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// Gets the LMS user id.
        /// </summary>
        public string lms_user_id
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.custom_canvas_user_id) || this.custom_canvas_user_id == "0" ? this.user_id : this.custom_canvas_user_id;
            }
        }

        /// <summary>
        /// Gets or sets the launch presentation return url.
        /// </summary>
        public string launch_presentation_return_url { get; set; }

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
        /// Gets or sets the LIS person source ID.
        /// </summary>
        public string lis_person_sourcedid { get; set; }

        /// <summary>
        /// Gets or sets the OAUTH consumer key.
        /// </summary>
        public string oauth_consumer_key { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public string roles { get; set; }

        /// <summary>
        /// Gets or sets the referer.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public string referer
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.refererField) ? HttpContext.Current.Request.Headers["Referer"] : this.refererField;
            }

            set
            {
                this.refererField = value;
            }
        }

        /// <summary>
        /// The LMS domain from uri.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string LmsDomainFromUrls()
        {
            if (string.IsNullOrWhiteSpace(this.referer))
            {
                return string.IsNullOrWhiteSpace(this.launch_presentation_return_url)
                           ? string.Empty
                           : new Uri(this.launch_presentation_return_url).GetLeftPart(UriPartial.Authority);
            }

            var scheme = new Uri(this.referer).GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
            var authority = new Uri(this.referer).GetLeftPart(UriPartial.Authority).ToLowerInvariant();
            return authority.Replace(scheme, string.Empty);
        }

        /// <summary>
        /// The try parse black board course id.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int TryParseBlackBoardCourseId()
        {
            int result = 0;
            if (this.tool_consumer_info_product_family_code.ToLowerInvariant().Contains("blackboard"))
            {
                result = this.GetCourseFromQueryOfUrl(this.launch_presentation_return_url);
                if (result == 0)
                {
                    result = this.GetCourseFromQueryOfUrl(this.referer);
                }
            }

            return result;
        }

        /// <summary>
        /// The get course from query of url.
        /// </summary>
        /// <param name="url">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetCourseFromQueryOfUrl(string url)
        {
            int result = 0;
            try
            {
                url = HttpUtility.UrlDecode(url);
                const string CourseIdQuery = "course_id";
                int index = url.IndexOf("?", StringComparison.Ordinal);
                if (index > 0)
                {
                    url = url.Substring(index).Remove(0, 1);
                }

                string courseId = HttpUtility.ParseQueryString(url).Get(CourseIdQuery);

                if (courseId != null)
                {
                    courseId = courseId.TrimStart('_').TrimEnd('1').TrimEnd('_');
                    if (int.TryParse(courseId, out result))
                    {
                        return result;
                    }
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return result;
        }

        #endregion
    }
}