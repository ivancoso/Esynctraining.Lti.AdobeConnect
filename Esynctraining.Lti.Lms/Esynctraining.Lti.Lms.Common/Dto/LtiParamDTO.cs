using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Esynctraining.Lti.Lms.Common.Constants;

namespace Esynctraining.Lti.Lms.Common.Dto
{
    public interface ILtiUserListParam
    {
        /// <summary>
        /// Gets the extended IMS LTI tool setting url. (sakai?)
        /// </summary>
        string ext_ims_lti_tool_setting_url { get; }

        /// <summary>
        /// Gets or sets the membership url. (sakai)
        /// </summary>
        string ext_ims_lis_memberships_url { get; }

        /// <summary>
        /// Gets the extended IMS LIST memberships id. (sakai)
        /// </summary>
        string ext_ims_lis_memberships_id { get; }


        // D2L + BB notification
        // calculated for UNIR API
        string lms_user_id { get; }

        // D2L
        string roles { get; }

        // D2L
        string ext_d2l_role { get; }

    }

    public interface ILtiParam : ILtiUserListParam
    {
        /// <summary>
        /// Gets the custom BlackBoard role.
        /// </summary>
        string custom_role { get; }

        /// <summary>
        /// Gets the course id.
        /// </summary>
        string course_id { get; }

        /// <summary>
        /// Gets the context label.
        /// </summary>
        string context_label { get; }

        /// <summary>
        /// Gets the context title.
        /// </summary>
        string context_title { get; }

        /// <summary>
        /// Gets the LMS user login id.
        /// </summary>
        string lms_user_login { get; }

        /// <summary>
        /// Gets the LIS person contact email primary.
        /// </summary>
        string lis_person_contact_email_primary { get; }

        string referer { get; }

        //string user_id { get; }

        string PersonNameGiven { get; }

        string PersonNameFamily { get; }

    }

    public class LtiParamDTO : ILtiParam
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
        public string course_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas API domain.
        /// </summary>
        public string custom_canvas_api_domain { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas course id.
        /// </summary>
        public string custom_canvas_course_id { get; set; }
        public string custom_live_course_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user id.
        /// </summary>
        public string custom_canvas_user_id { get; set; }

        /// <summary>
        /// Gets or sets the custom canvas user login id.
        /// </summary>
        public string custom_canvas_user_login_id { get; set; }

        /// <summary>
        /// UserName - Used by "schoology"
        /// </summary>
        public string custom_username { get; set; }

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

        // splitting according to recomendations https://www.imsglobal.org/wiki/step-3-are-all-required-parameters-present
        public string PersonNameFamily
        {
            get
            {
                if (!string.IsNullOrEmpty(lis_person_name_family))
                    return lis_person_name_family;
                if (!string.IsNullOrEmpty(lis_person_name_full))
                {
                    if (lis_person_name_full.Contains("@")) // canvas can return empty lis_person_name_family in case when user was created only with email, lis_person_name_full is filled
                        return lis_person_name_full.Split('@')[0];
                    var splitted = lis_person_name_full.Split(' ');
                    return splitted.Last();
                }

                return null;
            }
        }

        public string LastNameFromFullNameParam
        {
            get
            {
                if (!string.IsNullOrEmpty(lis_person_name_full))
                {
                    if (lis_person_name_full.Contains("@")) // canvas can return empty lis_person_name_family in case when user was created only with email, lis_person_name_full is filled
                        return lis_person_name_full.Split('@')[0];

                    var splitted = lis_person_name_full.Trim().Split(' ');
                    return splitted.Last();
                }
                return PersonNameFamily;
            }
        }


        /// <summary>
        /// Gets or sets the LIS person name full.
        /// </summary>
        public string lis_person_name_full { get; set; }

        /// <summary>
        /// Gets or sets the LIS person name given.
        /// </summary>
        public string lis_person_name_given { get; set; }

        public string PersonNameGiven
        {
            get
            {
                if (!string.IsNullOrEmpty(lis_person_name_given))
                    return lis_person_name_given;
                if (!string.IsNullOrEmpty(lis_person_name_full))
                {
                    var splitted = lis_person_name_full.Split(' ');
                    return splitted[0];
                }

                return null;
            }
        }

        public string FirstNameFromFullNameParam
        {
            get
            {
                if (!string.IsNullOrEmpty(lis_person_name_full))
                {
                    var splitted = lis_person_name_full.Trim().Split(' ');
                    return splitted[0];
                }
                return PersonNameGiven;
            }
        }

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
                string lmsDomain = GetLmsDomain().ToLower() ?? string.Empty;
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
                //custom_username:mike@esynctraining.com
                if (this.tool_consumer_info_product_family_code == "schoology")
                    return this.custom_username;

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
                return refererField;
            }

            set
            {
                refererField = value;
            }
        }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public string roles { get; set; }

        /// <summary>
        /// Gets or sets the custom BlackBoard role.
        /// </summary>
        public string custom_role { get; set; }

        /// <summary>
        /// Gets or sets the tool consumer info product family code.
        /// </summary>
        public string tool_consumer_info_product_family_code { get; set; }

        /// <summary>
        /// Gets or sets the custom AgilixBuzz domain.
        /// </summary>
        public string tool_consumer_instance_guid { get; set; }

        /// <summary>
        /// Gets or sets the LIST outcome service url.
        /// </summary>
        public string lis_outcome_service_url { get; set; }

        public string lis_result_sourcedid { get; set; }
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string user_id { get; set; }

        public string lti_version { get; set; }

        //[AllowHtml]
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
        //public string GetLtiProviderName(string externalProvider = null)
        //{
        //    string providerName = string.IsNullOrWhiteSpace(this.tool_consumer_info_product_family_code)
        //        ? externalProvider
        //        : this.tool_consumer_info_product_family_code.ToLower();
        //    if (externalProvider != null
        //        && externalProvider.Equals(LmsProviderNames.Blackboard, StringComparison.OrdinalIgnoreCase))
        //    {
        //        const string PatternToRemove = " learn";
        //        if (providerName.EndsWith(PatternToRemove, StringComparison.OrdinalIgnoreCase))
        //        {
        //            providerName = providerName.Replace(PatternToRemove, string.Empty);
        //        }
        //    }

        //    //if (externalProvider != null
        //    //    && externalProvider.Equals(LmsProviderNames.DialogEdu, StringComparison.OrdinalIgnoreCase))
        //    //{
        //    //    providerName = LmsProviderNames.DialogEdu;
        //    //}

        //    if (externalProvider != null
        //        && externalProvider.Equals(LmsProviderNames.Haiku, StringComparison.OrdinalIgnoreCase))
        //    {
        //        providerName = LmsProviderNames.Haiku;
        //    }

        //    // TRICK: for supporting old licenses
        //    if (providerName == "desire2learn")
        //        providerName = LmsProviderNames.Brightspace;

        //    return providerName;
        //}

        public string GetUserNameOrEmail()
        {
            return string.IsNullOrWhiteSpace(lms_user_login) ? lis_person_contact_email_primary : lms_user_login;
        }

        #endregion

        #region Methods

        private string GetCourseFromQueryOfUrl(string url)
        {
            try
            {
                url = HttpUtility.UrlDecode(url) ?? string.Empty;
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
                    if (long.TryParse(courseId, out long result))
                    {
                        return courseId;
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return null;
        }
        
        private string GetLmsDomain()
        {
            if (!string.IsNullOrWhiteSpace(this.custom_canvas_api_domain))
                return this.custom_canvas_api_domain;

            if (this.tool_consumer_info_product_family_code == "Buzz")
                return this.tool_consumer_instance_guid;

            if (this.tool_consumer_info_product_family_code == "schoology")
                return this.LmsDomainFromUrls();

            if (string.IsNullOrWhiteSpace(this.tool_consumer_instance_guid)
                || !string.IsNullOrWhiteSpace(this.lis_outcome_service_url)
                || !string.IsNullOrWhiteSpace(this.ext_ims_lis_memberships_url))
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
        
        private string LmsDomainFromUrls()
        {
            if (!string.IsNullOrWhiteSpace(this.lis_outcome_service_url))
            {
                return new Uri(this.lis_outcome_service_url).GetLeftPart(UriPartial.Authority);
            }

            // sakai
            if (!string.IsNullOrWhiteSpace(this.ext_ims_lis_memberships_url))
            {
                return new Uri(this.ext_ims_lis_memberships_url).GetLeftPart(UriPartial.Authority);
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

        public void CalculateFields()
        {
            course_id = CalcCourseId();
        }

        private string TryParseBlackBoardCourseId()
        {
            string result = null;
            if (tool_consumer_info_product_family_code != null &&
                tool_consumer_info_product_family_code.ToLowerInvariant().Contains("blackboard"))
            {
                result = GetCourseFromQueryOfUrl(launch_presentation_return_url);
                if (result == null)
                {
                    result = GetCourseFromQueryOfUrl(referer);
                }
            }

            return result;
        }
        
        private string TryGetSakaiCourseIdHash()
        {
            int result = 0;
            var code = tool_consumer_info_product_family_code;
            if (code != null && (code.ToLowerInvariant().Contains("sakai") || code.ToLowerInvariant().Contains("imsglc")))
            {
                Guid guid;
                if (Guid.TryParse(context_id, out guid))
                {
                    result = guid.GetHashCode();
                }
                else
                {
                    // TRICK: UNIR's Sakai returns non-guid values here
                    return context_id.GetHashCode().ToString();
                }
            }

            return result.ToString();
        }

        public string CalcCourseId()
        {
            if (tool_consumer_info_product_family_code != null &&
                tool_consumer_info_product_family_code.ToLowerInvariant().Contains("haiku"))
            {
                var contextId = Regex.Match(this.context_id, @"\d+").Value;

                return contextId;
            }

            if (custom_canvas_course_id != null)
            {
                return custom_canvas_course_id;
            }

            if (custom_live_course_id != null)
            {
                return custom_live_course_id;
            }

            if (long.TryParse(context_id, out long courseIdParse))
            {
                return context_id;
            }

            var courseId = TryParseBlackBoardCourseId();
            return courseId != null ? courseId : TryGetSakaiCourseIdHash();
        }

        #endregion

    }

}