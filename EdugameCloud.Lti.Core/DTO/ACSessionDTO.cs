namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Esynctraining.Core.Extensions;
    
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

        [ScriptIgnore]
        public DateTime? dateStarted { get; set; }

        [DataMember]
        public long? startedAt
        {
            get
            {
                if (!dateStarted.HasValue)
                    return null;
                return (long)dateStarted.Value.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

        [ScriptIgnore]
        public DateTime? dateEnded { get; set; }

        [DataMember]
        public long? endedAt
        {
            get
            {
                if (!dateEnded.HasValue)
                    return null;
                return (long)dateEnded.Value.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public int scoId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the meeting name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string meetingName { get; set; }

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public string sessionName { get; set; }

        /// <summary>
        /// Gets or sets the session index.
        /// </summary>
        [DataMember]
        //[ScriptIgnore]
        public int sessionNumber { get; set; }

        /// <summary>
        /// Gets or sets the asset(session) id.
        /// </summary>
        [DataMember]
        [ScriptIgnore]
        public int assetId { get; set; }

        #endregion

    }

}
