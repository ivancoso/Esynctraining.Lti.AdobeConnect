namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The SurveyQuestionResultAnswer DTO.
    /// </summary>
    [DataContract]
    public class SurveyQuestionResultAnswerDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultAnswerDTO"/> class.
        /// </summary>
        public SurveyQuestionResultAnswerDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultAnswerDTO"/> class. 
        /// </summary>
        /// <param name="answer">
        /// The result.
        /// </param>
        public SurveyQuestionResultAnswerDTO(SurveyQuestionResultAnswer answer)
        {
            this.surveyQuestionResultAnswerId = answer.Id;
            this.surveyQuestionResultId = answer.SurveyQuestionResult.With(x => x.Id);
            this.value = answer.Value;
            this.surveyDistractorId = answer.SurveyDistractor.Return(x => x.Id, (int?)null);
            this.surveyDistractorAnswerId = answer.SurveyDistractorAnswer.Return(x => x.Id, (int?)null);
            this.questionId = answer.SurveyQuestionResult.With(x => x.QuestionRef.Id);
            this.questionTypeId = answer.SurveyQuestionResult.With(x => x.QuestionType.Id);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember]
        public string value { get; set; }

        /// <summary>
        /// Gets or sets the survey question result id.
        /// </summary>
        [DataMember]
        public int surveyQuestionResultId { get; set; }

        /// <summary>
        /// Gets or sets the survey question result id.
        /// </summary>
        [DataMember]
        public int? surveyDistractorId { get; set; }

        /// <summary>
        /// Gets or sets the survey question result id.
        /// </summary>
        [DataMember]
        public int? surveyDistractorAnswerId { get; set; }

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
        public int surveyQuestionResultAnswerId { get; set; }

        #endregion
    }
}