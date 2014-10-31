namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The applet item DTO.
    /// </summary>
    [DataContract]
    public class ACSessionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionDTO"/> class.
        /// </summary>
        public ACSessionDTO()
        {
            this.participants = new List<ACSessionParticipantDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public List<ACSessionParticipantDTO> participants { get; set; }

        /// <summary>
        /// Gets or sets the date started.
        /// </summary>
        [DataMember]
        public DateTime? dateStarted { get; set; }

        /// <summary>
        /// Gets or sets the date ended.
        /// </summary>
        [DataMember]
        public DateTime? dateEnded { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [DataMember]
        public int scoId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the meeting name.
        /// </summary>
        [DataMember]
        public string meetingName { get; set; }

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        [DataMember]
        public string sessionName { get; set; }

        /// <summary>
        /// Gets or sets the session index.
        /// </summary>
        [DataMember]
        public int sessionNumber { get; set; }

        /// <summary>
        /// Gets or sets the asset(session) id.
        /// </summary>
        [DataMember]
        public int assetId { get; set; }

        #endregion
    }
}
