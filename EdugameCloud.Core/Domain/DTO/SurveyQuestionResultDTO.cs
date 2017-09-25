namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The Survey Question Result DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SurveyQuestionResultAnswerDTO))]
    public class SurveyQuestionResultDTO
    {
        /// <summary>
        /// The answers field.
        /// </summary>
        private SurveyQuestionResultAnswerDTO[] answersField = { };

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultDTO"/> class.
        /// </summary>
        public SurveyQuestionResultDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultDTO"/> class. 
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="answers">
        /// The answers.
        /// </param>
        public SurveyQuestionResultDTO(SurveyQuestionResult result, IEnumerable<SurveyQuestionResultAnswer> answers = null)
        {
            this.surveyQuestionResultId = result.Id;
            this.questionId = result.QuestionRef.With(x => x.Id);
            this.question = result.Question;
            this.questionTypeId = result.QuestionType.With(x => x.Id);
            this.isCorrect = result.IsCorrect;
            this.answers = answers.With(x => x.Select(a => new SurveyQuestionResultAnswerDTO(a)).ToArray());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public bool isArchive { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public SurveyQuestionResultAnswerDTO[] answers
        {
            get
            {
                return this.answersField ?? new SurveyQuestionResultAnswerDTO[] { };
            }

            set
            {
                this.answersField = value;
            }
        }

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
        /// Gets or sets the survey question result id.
        /// </summary>
        [DataMember]
        public int surveyQuestionResultId { get; set; }

        #endregion
    }
}