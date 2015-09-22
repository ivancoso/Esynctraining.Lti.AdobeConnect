namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    /// The crossword result DTO.
    /// </summary>
    [DataContract]
    public sealed class CrosswordResultByAcSessionDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordResultByAcSessionDTO"/> class.
        /// </summary>
        public CrosswordResultByAcSessionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordResultByAcSessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public CrosswordResultByAcSessionDTO(CrosswordResultByAcSessionFromStoredProcedureDTO dto)
        {
            this.documentXML = dto.documentXML;
            this.position = dto.position;
            this.appletResultId = dto.appletResultId;
            this.endTime = dto.endTime.ConvertToUnixTimestamp();
            this.startTime = dto.startTime.ConvertToUnixTimestamp();
            this.participantName = dto.participantName;
            this.position = dto.position;
            this.score = dto.score;
        }

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
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

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
        /// Gets or sets the start time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double startTime { get; set; }

        #endregion
    }
}