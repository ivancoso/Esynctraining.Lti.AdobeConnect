namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The participant DTO.
    /// </summary>
    [DataContract]
    // ReSharper disable once InconsistentNaming
    public class ACSessionParticipantDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the ac session participant id.
        /// </summary>
        [DataMember]
        public int acSessionParticipantId { get; set; }

        /// <summary>
        ///     Gets or sets the date entered.
        /// </summary>
        [DataMember]
        public DateTime dateTimeEntered { get; set; }

        /// <summary>
        ///     Gets or sets the date left.
        /// </summary>
        [DataMember]
        public DateTime dateTimeLeft { get; set; }

        /// <summary>
        ///     Gets or sets the ac session participant id.
        /// </summary>
        [DataMember]
        public float durationInHours { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        [DataMember]
        public string login { get; set; }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the transcript id.
        /// </summary>
        public int transcriptId { get; set; }

        #endregion
    }
}
