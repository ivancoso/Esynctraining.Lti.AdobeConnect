namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The SN session member.
    /// </summary>
    [DataContract]
    public class SNMemberDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNMemberDTO"/> class.
        /// </summary>
        public SNMemberDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMemberDTO"/> class.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        public SNMemberDTO(SNMember member)
        {
            if (member != null)
            {
                this.acSessionId = member.ACSessionId;
                this.snMemberId = member.Id;
                this.participant = member.Participant;
                this.participantProfile = member.ParticipantProfile;
                this.dateCreated = (member.DateCreated ?? DateTime.Now).ConvertToUnixTimestamp();
                this.isBlocked = member.IsBlocked;
            }
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the SB session member id.
        /// </summary>
        [DataMember]
        public int snMemberId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string participant { get; set; }

        /// <summary>
        /// Gets or sets the participant profile.
        /// </summary>
        [DataMember]
        public string participantProfile { get; set; }

        /// <summary>
        /// Gets or sets the date created. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is blocked.
        /// </summary>
        [DataMember]
        public bool isBlocked { get; set; }

        #endregion
    }
}