﻿namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyResultDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SurveyResultDTO(SurveyResult result)
        {
            this.surveyResultId = result.Id;
            this.acSessionId = result.ACSessionId;
            this.surveyId = result.Survey.With(x => x.Id);
            this.dateCreated = result.DateCreated.ConvertToUnixTimestamp();
            this.endTime = result.EndTime.ConvertToUnixTimestamp();
            this.participantName = result.ParticipantName;
            this.score = result.Score;
            this.startTime = result.StartTime.ConvertToUnixTimestamp();
            this.isArchive = result.IsArchive ?? false;
            this.email = result.Email;
            this.lmsUserParametersId = result.LmsUserParametersId ?? 0;
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
        /// Gets or sets the applet name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int surveyId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int companyId { get; set; }

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

        #endregion
    }
}