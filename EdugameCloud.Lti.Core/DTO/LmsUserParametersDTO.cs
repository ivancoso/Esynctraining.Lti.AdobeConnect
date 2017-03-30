namespace EdugameCloud.Lti.DTO
{
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
            this.lmsUserParametersId = param.Id;
            this.acId = param.AcId;
            this.course = param.Course;
            this.domain = param.CompanyLms.LmsDomain;
            this.provider = lmsProvider.ShortName;
            this.wstoken = param.Wstoken;
            this.lmsUserId = param.LmsUser.Return(x => x.Id, (int?)null);
            this.courseName = param.CourseName;
            this.userEmail = param.UserEmail;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public int lmsUserParametersId { get; set; }

        [DataMember]
        public string acId { get; set; }

        [DataMember]
        public int course { get; set; }

        [DataMember]
        public string domain { get; set; }

        [DataMember]
        public string errorDetails { get; set; }

        [DataMember]
        public string errorMessage { get; set; }

        [DataMember]
        public int? lmsUserId { get; set; }

        [DataMember]
        public string provider { get; set; }

        [DataMember]
        public string wstoken { get; set; }

        [DataMember]
        public string userEmail { get; set; }

        [DataMember]
        public string courseName { get; set; }

        #endregion

    }

}