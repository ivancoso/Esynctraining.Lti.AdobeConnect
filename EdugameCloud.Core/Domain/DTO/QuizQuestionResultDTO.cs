namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The quiz question result DTO.
    /// </summary>
    [DataContract]
    public class QuizQuestionResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionResultDTO"/> class.
        /// </summary>
        public QuizQuestionResultDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionResultDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public QuizQuestionResultDTO(QuizQuestionResult result)
        {
            this.quizQuestionResultId = result.Id;
            this.quizResultId = result.QuizResult.With(x => x.Id);
            this.questionId = result.QuestionRef.With(x => x.Id);
            this.question = result.Question;
            this.questionTypeId = result.QuestionType.With(x => x.Id);
            this.isCorrect = result.IsCorrect;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        [DataMember]
        public bool isCorrect { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        [DataMember]
        public string question { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public int questionId { get; set; }

        /// <summary>
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public int questionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the quiz question result id.
        /// </summary>
        [DataMember]
        public int quizQuestionResultId { get; set; }

        /// <summary>
        /// Gets or sets the quiz result id.
        /// </summary>
        [DataMember]
        public int quizResultId { get; set; }

        #endregion
    }
}