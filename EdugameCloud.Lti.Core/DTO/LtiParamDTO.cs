namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The LTI parameter DTO.
    /// </summary>
    public class LtiParamDTO
    {
        #region Fields

        /// <summary>
        ///     The referer.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        private string refererField;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the context id. (general)
        /// </summary>
        public string context_id { get; set; }

        /// <summary>
        /// Gets or sets the membership url. (sakai)
        /// </summary>
        public string ext_ims_lis_memberships_url { get; set; }

        /// <summary>
        /// Gets or sets the context label.
        /// </summary>
        public string context_label { get; set; }

        /// <summary>
        /// Gets or sets the context title.
        /// </summary>
        public string context_title { get; set; }

        /// <summary>
        /// Gets the course id.
        /// </summary>
        public int course_id
        {
            get
            {
                if (this.custom_canvas_course_id != 0)
                {
                    return this.custom_canvas_course_id;
                }

                int courseId;
                if (int.TryParse(this.context_id, out courseId))
                {
                    return courseId;
                }

                courseId = this.TryParseBlackBoardCourseId();
                return courseId != 0 ? courseId : this.TryGetSakaiCourseIdHash();
            }
        }

        /// <summary>
        /// Gets or sets the custom canvas API domain.
        /// </summary>
        public string custom_canvas_api_domain { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas course id.
        /// </summary>
        public int custom_canvas_course_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user id.
        /// </summary>
        public string custom_canvas_user_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user login id.
        /// </summary>
        public string custom_canvas_user_login_id { get; set; }

        /// <summary>
        /// Gets or sets the extended IMS LIST memberships id. (sakai)
        /// </summary>
        public string ext_ims_lis_memberships_id { get; set; }

        /// <summary>
        /// Gets or sets the extended IMS LTI tool setting url. (sakai?)
        /// </summary>
        public string ext_ims_lti_tool_setting_url { get; set; }

        public string ext_user_username { get; set; }

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
        /// Gets or sets the custom_enablemeeting.
        /// </summary>
        public string custom_enablemeeting { get; set; }

        public string custom_enablestudygroup { get; set; }

        /// <summary>
        /// Gets a value indicating whether is_course_meeting_enabled.
        /// </summary>
        public bool is_course_meeting_enabled
        {
            get
            {
                return this.custom_enablemeeting != null && this.custom_enablemeeting.ToUpper().Equals("TRUE");
            }
        }

        public bool? is_course_study_group_enabled
        {
            get
            {
                if (custom_enablestudygroup == null)
                    return null;
                if (custom_enablestudygroup.ToUpper().Equals("FALSE"))
                    return false;
                if (custom_enablestudygroup.ToUpper().Equals("TRUE"))
                    return true;
                return null;
            }
        }

        /// <summary>
        /// Gets the LMS domain.
        /// </summary>
        public string lms_domain
        {
            get
            {
                string lmsDomain = this.GetLmsDomain().Return(x => x.ToLower(), string.Empty);
                if (lmsDomain.StartsWith(HttpScheme.Http))
                {
                    return lmsDomain.Substring(HttpScheme.Http.Length);
                }

                if (lmsDomain.StartsWith(HttpScheme.Https))
                {
                    return lmsDomain.Substring(HttpScheme.Https.Length);
                }

                return lmsDomain;
            }
        }

        /// <summary>
        /// Gets the LMS user id.
        /// </summary>
        public string lms_user_id
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.custom_canvas_user_id) || this.custom_canvas_user_id == "0"
                           ? this.user_id
                           : this.custom_canvas_user_id;
            }
        }

        /// <summary>
        /// Gets the LMS user login id.
        /// </summary>
        public string lms_user_login
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.custom_canvas_user_login_id)
                            ? (string.IsNullOrWhiteSpace(ext_d2l_username) 
                                ? (string.IsNullOrWhiteSpace(this.ext_user_username)
                                    ? this.lis_person_sourcedid
                                    : this.ext_user_username)
                                : ext_d2l_username)
                            : this.custom_canvas_user_login_id;
            }
        }

        /// <summary>
        /// Gets or sets the OAUTH consumer key.
        /// </summary>
        public string oauth_consumer_key { get; set; }

        /// <summary>
        /// Gets or sets the referer.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        public string referer
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.refererField)
                           ? HttpContext.Current.Request.Headers["Referer"]
                           : this.refererField;
            }

            set
            {
                this.refererField = value;
            }
        }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public string roles { get; set; }

        /// <summary>
        /// Gets or sets the tool consumer info product family code.
        /// </summary>
        public string tool_consumer_info_product_family_code { get; set; }

        /// <summary>
        /// Gets or sets the custom brain honey domain.
        /// </summary>
        public string tool_consumer_instance_guid { get; set; }

        /// <summary>
        /// Gets or sets the LIST outcome service url.
        /// </summary>
        public string lis_outcome_service_url { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string user_id { get; set; }

        [AllowHtml]
        public string resource_link_description { get; set; }

        // D2L properties
        public string ext_d2l_username { get; set; }
        public string ext_d2l_orgdefinedid { get; set; }
        public string ext_d2l_role { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get LTI provider name.
        /// </summary>
        /// <param name="externalProvider">
        /// The external provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetLtiProviderName(string externalProvider = null)
        {
            string providerName = string.IsNullOrWhiteSpace(this.tool_consumer_info_product_family_code)
                                      ? externalProvider
                                      : this.tool_consumer_info_product_family_code.ToLower();
            if (externalProvider != null
                && externalProvider.Equals(LmsProviderNames.Blackboard, StringComparison.OrdinalIgnoreCase))
            {
                const string PatternToRemove = " learn";
                if (providerName.EndsWith(PatternToRemove, StringComparison.OrdinalIgnoreCase))
                {
                    providerName = providerName.Replace(PatternToRemove, string.Empty);
                }
            }

            // TRICK: for supporting old licenses
            if (providerName == "desire2learn")
                providerName = LmsProviderNames.Brightspace;

            return providerName;
        }

        #endregion

        #region Methods

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
                url = HttpUtility.UrlDecode(url).Return(x => x, string.Empty);
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

        /// <summary>
        ///     The get LMS domain.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GetLmsDomain()
        {
            if (string.IsNullOrWhiteSpace(this.custom_canvas_api_domain))
            {
                if (string.IsNullOrWhiteSpace(this.tool_consumer_instance_guid)
                    || !string.IsNullOrWhiteSpace(this.lis_outcome_service_url))
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

        /// <summary>
        ///     The LMS domain from uri.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string LmsDomainFromUrls()
        {
            if (!string.IsNullOrWhiteSpace(this.lis_outcome_service_url))
            {
                return new Uri(this.lis_outcome_service_url).GetLeftPart(UriPartial.Authority);
            }

            if (string.IsNullOrWhiteSpace(this.referer))
            {
                return string.IsNullOrWhiteSpace(this.launch_presentation_return_url)
                           ? string.Empty
                           : new Uri(this.launch_presentation_return_url).GetLeftPart(UriPartial.Authority);
            }

            string scheme = new Uri(this.referer).GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
            string authority = new Uri(this.referer).GetLeftPart(UriPartial.Authority).ToLowerInvariant();
            return authority.Replace(scheme, string.Empty);
        }

        /// <summary>
        ///     The try parse black board course id.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        private int TryParseBlackBoardCourseId()
        {
            int result = 0;
            if (this.tool_consumer_info_product_family_code.Return(x => x.ToLowerInvariant().Contains("blackboard"), false))
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
        /// The try get sakai course id hash.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int TryGetSakaiCourseIdHash()
        {
            int result = 0;

            if (this.tool_consumer_info_product_family_code.Return(x => x.ToLowerInvariant().Contains("sakai"), false))
            {
                Guid guid;
                if (Guid.TryParse(this.context_id, out guid))
                {
                    result = guid.GetHashCode();
                }
                else
                {
                    // TRICK: UNIR's Sakai returns non-guid values here
                    return this.context_id.GetHashCode();
                }
            }

            return result;
        }

        #endregion
    }
}