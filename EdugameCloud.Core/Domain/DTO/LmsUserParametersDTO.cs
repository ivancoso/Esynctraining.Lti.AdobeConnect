namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersDTO"/> class.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        public LmsUserParametersDTO(LmsUserParameters param)
        {
            this.lmsUserParametersId = param.Id;
            this.acId = param.AcId;
            this.course = param.Course;
            this.domain = param.CompanyLms.LmsDomain;
            this.provider = param.CompanyLms.LmsProvider.ShortName;
            this.wstoken = param.Wstoken;
            this.lmsUserId = param.LmsUser.Return(x => x.Id, (int?)null);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the lms user parameters id.
        /// </summary>
        [DataMember]
        public int lmsUserParametersId { get; set; }

        /// <summary>
        /// Gets or sets the ac id.
        /// </summary>
        [DataMember]
        public string acId { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        [DataMember]
        public int course { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [DataMember]
        public string domain { get; set; }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        [DataMember]
        public string errorDetails { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember]
        public string errorMessage { get; set; }

        /// <summary>
        /// Gets or sets the LMS user id.
        /// </summary>
        [DataMember]
        public int? lmsUserId { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [DataMember]
        public string provider { get; set; }

        /// <summary>
        /// Gets or sets the WS token.
        /// </summary>
        [DataMember]
        public string wstoken { get; set; }

        #endregion
    }
}