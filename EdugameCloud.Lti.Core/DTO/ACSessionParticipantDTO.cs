namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Esynctraining.Core.Extensions;

    [DataContract]
    public class ACSessionParticipantDTO : IComparable<ACSessionParticipantDTO>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the sco id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string scoId  { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the sco name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string scoName { get; set; }

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string sessionName { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string principalId { get; set; }

        /// <summary>
        /// Gets or sets the asset id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string assetId { get; set; }

        [ScriptIgnore]
        public DateTime dateTimeEntered { get; set; }

        [DataMember]
        public long enteredAt
        {
            get
            {
                return (long)dateTimeEntered.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

        [ScriptIgnore]
        public DateTime? dateTimeLeft { get; set; }

        [DataMember]
        public long? leftAt
        {
            get
            {
                if (!dateTimeLeft.HasValue)
                    return null;
                return (long)dateTimeLeft.Value.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

        [DataMember]
        [ScriptIgnore]
        public float durationInHours { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string login { get; set; }

        /// <summary>
        /// Gets login of full name.
        /// </summary>
        [ScriptIgnore]
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
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the transcript id.
        /// </summary>
        [ScriptIgnore]
        public string transcriptId { get; set; }

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
