namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The survey session DTO.
    /// </summary>
    [DataContract]
    public class SurveySessionFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public virtual int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the total score.
        /// </summary>
        [DataMember]
        public virtual int TotalScore { get; set; }

        /// <summary>
        /// Gets or sets the ac session id.
        /// </summary>
        [DataMember]
        public virtual int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the ac user mode id.
        /// </summary>
        [DataMember]
        public virtual int acUserModeId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public virtual string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [IgnoreDataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public virtual string language { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public virtual int activeParticipants { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public virtual int totalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public virtual int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the survey name.
        /// </summary>
        [DataMember]
        public virtual string surveyName { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        #endregion
    }
}