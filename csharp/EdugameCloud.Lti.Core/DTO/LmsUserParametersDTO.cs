namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The LMS user parameters DTO.
    /// </summary>
    [DataContract]
    public class LmsUserParametersDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersDTO"/> class.
        /// </summary>
        public LmsUserParametersDTO()
        {
        }

        public LmsUserParametersDTO(LmsUserParameters param, LmsProvider lmsProvider)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (lmsProvider == null)
                throw new ArgumentNullException(nameof(lmsProvider));

            LmsUserParametersId = param.Id;
            AcId = param.AcId;
            Course = param.Course;
            Domain = param.CompanyLms.LmsDomain;
            provider = lmsProvider.ShortName;
            WsToken = param.Wstoken;
            LmsUserId = param.LmsUser.Return(x => x.Id, (int?)null);
            CourseName = param.CourseName;
            UserEmail = param.UserEmail;
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "lmsUserParametersId")]
        public int LmsUserParametersId { get; set; }

        [DataMember(Name = "acId")]
        public string AcId { get; set; }

        [DataMember(Name = "course")]
        public string Course { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "errorDetails")]
        public string ErrorDetails { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "lmsUserId")]
        public int? LmsUserId { get; set; }

        [DataMember(Name = "provider")]
        public string provider { get; set; }

        [DataMember(Name = "wstoken")]
        public string WsToken { get; set; }

        [DataMember(Name = "userEmail")]
        public string UserEmail { get; set; }

        [DataMember(Name = "courseName")]
        public string CourseName { get; set; }

        #endregion

    }

}