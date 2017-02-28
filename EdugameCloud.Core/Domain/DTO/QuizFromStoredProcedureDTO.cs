namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The quiz item DTO.
    /// </summary>
    [DataContract]
    public class QuizFromStoredProcedureDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFromStoredProcedureDTO"/> class.
        /// </summary>
        public QuizFromStoredProcedureDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFromStoredProcedureDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public QuizFromStoredProcedureDTO(QuizFromStoredProcedureExDTO dto)
        {
            this.quizId = dto.quizId;
            this.quizName = dto.quizName;
            this.categoryName = dto.categoryName;
            this.createdBy = dto.createdBy;
            this.createdByLastName = dto.createdByLastName;
            this.createdByName = dto.createdByName;
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.firstName = dto.firstName;
            this.lastName = dto.lastName;
            this.description = dto.description;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleItemId = dto.subModuleItemId;
            this.userId = dto.userId;
            this.lmsQuizId = dto.lmsQuizId;
            isPostQuiz = dto.isPostQuiz;
            PassingScore = dto.PassingScore;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        [DataMember]
        public int quizId { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string quizName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the created by last name.
        /// </summary>
        [DataMember]
        public string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets the created by name.
        /// </summary>
        [DataMember]
        public string createdByName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the LMS quiz id.
        /// </summary>
        [DataMember]
        public int? lmsQuizId { get; set; }

        [DataMember]
        public bool isPostQuiz { get; set; }

        [DataMember(Name = "passingScore")]
        public virtual int PassingScore { get; set; }

        #endregion
    }
}