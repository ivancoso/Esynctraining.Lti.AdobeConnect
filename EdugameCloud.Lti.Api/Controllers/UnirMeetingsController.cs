using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.GetHashCodeTrick;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class UnirMeetingsController : BaseApiController
    {
        public class ApiLtiParam : ILtiParam
        {
            // BB only.
            [IgnoreDataMember]
            public string custom_role { get; set; } = "";

            // TODO: calculated (Canvas=custom_canvas_course_id)
            // TODO: !!
            [IgnoreDataMember]
            public int course_id
            {
                get
                {
                    if (tool_consumer_info_product_family_code == "canvas")
                        return int.Parse(course);

                    if (tool_consumer_info_product_family_code == "sakai")
                        return SakaiCourseNumberTrick.GetHashCode(course);

                    throw new NotSupportedException($"{tool_consumer_info_product_family_code} is not supported LMS for this API");

                    //if (custom_canvas_course_id != 0)1
                    //    return custom_canvas_course_id;

                    //if (context_id != null)
                    //    return SakaiCourseNumberTrick.GetHashCode(context_id);

                    //return -1;
                }
            }

            /// <summary>
            /// Canvas: custom_canvas_course_id;
            /// Sakai: context_id.
            /// </summary>
            [Required]
            public string course { get; set; }

            ///// <summary>
            ///// course id (canvas)
            ///// </summary>
            //public int custom_canvas_course_id { get; set; }

            ///// <summary>
            ///// Used as course id (sakai)
            ///// </summary>
            //public string context_id { get; set; }

            [IgnoreDataMember]
            public string context_label { get; set; }

            [IgnoreDataMember]
            public string context_title { get; set; }

            /// <summary>
            /// LTI: 'context_label' or 'context_title' form value.
            /// </summary>
            [Required]
            public string course_name
            {
                get { return context_label; }
                set { context_label = value; context_title = value; }
            }

            /// <summary>
            /// Canvas: custom_canvas_user_login_id
            /// Sakai: lis_person_sourcedid
            /// </summary>
            [IgnoreDataMember]
            public string lms_user_login { get; set; }
            //{
            //    get
            //    {
            //        // canvas
            //        if (!string.IsNullOrWhiteSpace(custom_canvas_user_login_id))
            //            return custom_canvas_user_login_id;

            //        // sakai
            //        return lis_person_sourcedid;
            //    }
            //}

            /// <summary>
            /// (sakai)
            /// </summary>
            //public string lis_person_sourcedid { get; set; }

            //public string custom_canvas_user_login_id { get; set; }

            [Required]
            public string lis_person_contact_email_primary { get; set; }

            /// <summary>
            /// LTI tool absolute URL.
            /// Used within Announcement message email.
            /// </summary>
            public string referer { get; set; }

            // TODO: calculated
            // Canvas = lis_person_name_given
            [IgnoreDataMember]
            public string PersonNameGiven => lis_person_name_given;

            [Required]
            public string lis_person_name_given { get; set; }

            // TODO: calculated
            // Canvas = lis_person_name_family
            [IgnoreDataMember]
            public string PersonNameFamily => lis_person_name_family;

            [Required]
            public string lis_person_name_family { get; set; }

            /// <summary>
            /// Gets the extended IMS LTI tool setting url. (sakai)
            /// </summary>
            [IgnoreDataMember]
            public string ext_ims_lti_tool_setting_url { get; set; }

            /// <summary>
            /// Gets or sets the membership url. (sakai)
            /// </summary>
            public string ext_ims_lis_memberships_url { get; set; }

            /// <summary>
            /// Gets the extended IMS LIST memberships id. (sakai)
            /// </summary>
            public string ext_ims_lis_memberships_id { get; set; }

            /// <summary>
            /// Canvas LTI request form value: custom_canvas_user_id;
            /// Sakai LTI request form value: user_id.
            /// </summary>
            [Required]
            public string lms_user_id { get; set; }
            //{
            //    get
            //    {
            //        return string.IsNullOrWhiteSpace(this.custom_canvas_user_id) || this.custom_canvas_user_id == "0"
            //            ? this.user_id
            //            : this.custom_canvas_user_id;

            //    }
            //}

            /// <summary>
            /// Canvas LTI: custom_canvas_user_id
            /// Sakai LTI: user_id
            /// </summary>
            //public string user_id { get; set; }

            //public string custom_canvas_user_id { get; set; }

            /// <summary>
            /// Possible values: canvas; sakai
            /// </summary>
            [Required]
            public string tool_consumer_info_product_family_code { get; set; }


            [IgnoreDataMember]
            public string roles { get; set; } = "";

            [IgnoreDataMember]
            public string ext_d2l_role { get; set; } = "";

        }


        [DataContract]
        public class CreateMeetingDto
        {
            [Required]
            [DataMember]
            public ApiLtiParam ApiLtiParam { get; set; }

            /// <summary>
            /// Format: "02-14-2019"
            /// </summary>
            [Required]
            [DataMember]
            public string StartDate { get; set; }

            /// <summary>
            /// Format: "03:30 PM"
            /// </summary>
            [Required]
            [DataMember]
            public string StartTime { get; set; }

            /// <summary>
            /// Custom URL.
            /// Leave this field blank for a system-generated URL, or include a unique URL path. 
            /// Please use only ascii alphanumeric characters or hyphens.
            /// </summary>
            [DataMember]
            public string AcRoomUrl { get; set; }

            /// <summary>
            /// Format: "02:30"
            /// </summary>
            [Required]
            [DataMember]
            public string Duration { get; set; }

            [DataMember]
            [Required]
            public string Name { get; set; }

            /// <summary>
            /// max length=4000 characters
            /// </summary>
            [DataMember]
            public string Summary { get; set; }

            /// <summary>
            /// sco-id of AdobeConnect meeting template.
            /// </summary>
            [DataMember]
            [Required]
            public string Template { get; set; }


            public MeetingDTOInput Build()
            {
                return new MeetingDTOInput
                {
                    AccessLevel = "view_hidden", // TODO???
                    AcRoomUrl = AcRoomUrl,
                    //AudioProfileId
                    //AudioProfileName
                    //CanJoin
                    //ClassRoomId
                    Duration = Duration,
                    //Id
                    //IsDisabledForThisCourse
                    //IsEditable
                    Name = Name,
                    //OfficeHours
                    //Reused
                    //ReusedByAnotherMeeting
                    //SectionIds
                    //Sessions
                    StartDate = StartDate,
                    StartTime = StartTime,
                    //StartTimeStamp
                    Summary = Summary,
                    //TelephonyProfileFields
                    Template = Template,
                    Type = (int)LmsMeetingType.Meeting,
                };
            }

        }

        private readonly MeetingSetup _meetingSetup;


        public UnirMeetingsController(
            MeetingSetup meetingSetup,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            _meetingSetup = meetingSetup;
        }


        [Route("v1/meeting")]
        [HttpPost]
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> CreateNewMeeting([FromBody]CreateMeetingDto input)
        {
            try
            {
                MeetingDTOInput meeting = input.Build();

                var trace = new StringBuilder();
                var ac = this.GetAdminProvider();
                //// TRICK: new don't care about this settings - cause there is a check UseUserFolder==false
                //bool useLmsUserEmailForSearch = true;
                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(input.ApiLtiParam.lis_person_contact_email_primary);
                var fb = new MeetingFolderBuilder((LmsCompany)LmsCompany, ac, useLmsUserEmailForSearch, LmsMeetingType.Meeting);

                /*
                // tool_consumer_info_product_family_code : canvas
                // https://esynctraining.instructure.com/courses/231/external_tools/84
                var apiTest = input.ApiLtiParam;
                apiTest.tool_consumer_info_product_family_code = "canvas";
                apiTest.course = "231";
                apiTest.course_name= "Regression";
                //apiTest.context_label = "Regression";
                //apiTest.context_title = "Regression";

                apiTest.lms_user_login = "mike@esynctraining.com"; // custom_canvas_user_login_id
                apiTest.lis_person_contact_email_primary = "mike@esynctraining.com";

                apiTest.referer = "https://esynctraining.instructure.com/courses/231/external_tools/84";
                apiTest.lms_user_id = "1"; // custom_canvas_user_id 
                
                apiTest.lis_person_name_given = "Mike";
                apiTest.lis_person_name_family = "Kollen";
                */


                // tool_consumer_info_product_family_code: sakai
                // SAKAI: 091887fe-a54e-47d6-8e2a-f386f1d24426
                // course id = test_lti

                /*var apiTest = input.ApiLtiParam;
                apiTest.course = "test_lti";
                apiTest.tool_consumer_info_product_family_code = "sakai";
                apiTest.course_name = "Test LTI site";
                //apiTest.context_label = "Test LTI site";
                //apiTest.context_title = "Test LTI site";
                apiTest.lis_person_contact_email_primary = "vadim+sakai11@esynctraining.com";
                // ??? HEADER??
                apiTest.referer = "http://sakai11.esynctraining.com/access/basiclti/site/test_lti/e8efacd7-f9eb-4cd5-9993-07d992666617";

                apiTest.lms_user_login = "admin"; // sakai: lis_person_sourcedid
                apiTest.lms_user_id = "admin"; // sakai: user_id 

                apiTest.lis_person_name_given = "Sakai";
                apiTest.lis_person_name_family = "Administrator";

                apiTest.ext_ims_lis_memberships_url = "http://sakai11.esynctraining.com/imsblis/service/";
                //apiTest.ext_ims_lti_tool_setting_url = "http://sakai11.esynctraining.com/imsblis/service/";
                apiTest.ext_ims_lis_memberships_id = "e4edb1c55e145b06d1390111d5d5590689834fe2a1882f301b10f397a89627bb:::admin:::e8efacd7-f9eb-4cd5-9993-07d992666617";
                */

                OperationResult ret = await _meetingSetup.SaveMeeting(
                    LmsCompany,
                    ac,
                    input.ApiLtiParam,
                    meeting,
                    trace,
                    fb);

                return ret;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeeting", ex);
                return OperationResult.Error(errorMessage);
            }
        }

    }

}