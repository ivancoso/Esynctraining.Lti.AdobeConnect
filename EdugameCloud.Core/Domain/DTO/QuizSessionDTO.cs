namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using Esynctraining.Core.Extensions;

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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            acSessionId = dto.acSessionId;
            acUserModeId = dto.acUserModeId;
            categoryName = dto.categoryName;
            dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            language = dto.language;
            activeParticipants = dto.activeParticipants;
            totalParticipants = dto.totalParticipants;
            quizName = dto.quizName;
            subModuleItemId = dto.subModuleItemId;
            eventQuizMappingId = dto.eventQuizMappingId;
        }

        #region Public Properties
                   
        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string quizName { get; set; }

        [DataMember]
        public int? eventQuizMappingId { get; set; }

        #endregion

    }

}