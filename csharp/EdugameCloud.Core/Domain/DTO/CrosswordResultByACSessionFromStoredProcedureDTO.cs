namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The crossword result DTO.
    /// </summary>
    [DataContract]
    public class CrosswordResultByAcSessionFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the document xml.
        /// </summary>
        [DataMember]
        public string documentXML { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public long position { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        [DataMember]
        public DateTime endTime { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int appletResultId { get; set; }

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
        /// Gets or sets the start time.
        /// </summary>
        [DataMember]
        public DateTime startTime { get; set; }

        #endregion
    }
}