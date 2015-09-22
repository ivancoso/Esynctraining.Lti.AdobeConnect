namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Extensions;

    /// <summary>
    /// The quiz session DTO.
    /// </summary>
    [DataContract]
    public sealed class QuizSessionDTO : AdobeConnectSessionDtoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSessionDTO"/> class.
        /// </summary>
        public QuizSessionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public QuizSessionDTO(QuizSessionFromStoredProcedureDTO dto)
        {
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.language = dto.language;
            this.activeParticipants = dto.activeParticipants;
            this.totalParticipants = dto.totalParticipants;
            this.quizName = dto.quizName;
            this.subModuleItemId = dto.subModuleItemId;
        }

        #region Public Properties
                   
        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string quizName { get; set; }
                
        #endregion

    }

}