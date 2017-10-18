namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    // TODO: move to separate file
    [DataContract]
    public class SurveyResultSaveResultDTO : SurveyResultDTO
    {
        [DataMember]
        public SurveyQuestionResultSaveAllDTO surveyQuestionResult { get; set; }
    }

    /// <summary>
    ///     The survey result DTO.
    /// </summary>
    [DataContract]
    public class SurveyResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyResultDTO"/> class.
        /// </summary>
        public SurveyResultDTO()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public bool isArchive { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int surveyResultId { get; set; }

        /// <summary>
        /// Gets or sets the participant name
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public int score { get; set; }

        /// <summary>
        /// Gets or sets the start time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double startTime { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters id.
        /// </summary>
        [DataMember]
        public int lmsUserParametersId { get; set; }

        [DataMember]
        public string acEmail { get; set; }

        [DataMember]
        public SurveyQuestionResultDTO[] results { get; set; }

        public int acSessionId { get; set; }

        #endregion
    }
}