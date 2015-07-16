namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The participant DTO.
    /// </summary>
    [DataContract]
    // ReSharper disable once InconsistentNaming
    public class ACSessionParticipantDTO : IComparable<ACSessionParticipantDTO>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the sco id.
        /// </summary>
        [DataMember]
        public string scoId  { get; set; }

        /// <summary>
        ///     Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        ///     Gets or sets the sco name.
        /// </summary>
        [DataMember]
        public string scoName { get; set; }

        /// <summary>
        ///     Gets or sets the session name.
        /// </summary>
        [DataMember]
        public string sessionName{ get; set; }

        /// <summary>
        ///     Gets or sets the principal id.
        /// </summary>
        [DataMember]
        public string principalId { get; set; }

        /// <summary>
        ///     Gets or sets the asset id.
        /// </summary>
        [DataMember]
        public string assetId { get; set; }

        /// <summary>
        ///     Gets or sets the date entered.
        /// </summary>
        [DataMember]
        public DateTime dateTimeEntered { get; set; }

        /// <summary>
        ///     Gets or sets the date left.
        /// </summary>
        [DataMember]
        public DateTime? dateTimeLeft { get; set; }

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
        ///     Gets login of full name.
        /// </summary>
        public string loginOrFullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.login) ? this.login : this.fullName;
            }
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string fullName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.lastName) ? this.firstName : string.Format("{0} {1}", this.firstName, this.lastName);
            }
        }

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

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(ACSessionParticipantDTO other)
        {
            return string.Compare(this.loginOrFullName, other.loginOrFullName, StringComparison.Ordinal);
        }
    }
}
