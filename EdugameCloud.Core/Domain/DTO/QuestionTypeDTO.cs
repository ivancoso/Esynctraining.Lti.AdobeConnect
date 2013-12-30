namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The question type dto.
    /// </summary>
    [DataContract]
    public class QuestionTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionTypeDTO"/> class.
        /// </summary>
        public QuestionTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionTypeDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public QuestionTypeDTO(QuestionType result)
        {
            this.questionTypeId = result.Id;
            this.questionTypeOrder = result.QuestionTypeOrder;
            this.type = result.Type;
            this.questionTypeDescription = result.QuestionTypeDescription;
            this.instruction = result.Instruction;
            this.correctText = result.CorrectText;
            this.incorrectMessage = result.IncorrectMessage;
            this.isActive = result.IsActive;
            this.iconSource = result.IconSource;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the correct text.
        /// </summary>
        [DataMember]
        public string correctText { get; set; }

        /// <summary>
        /// Gets or sets the incorrect message.
        /// </summary>
        [DataMember]
        public string incorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the icon source.
        /// </summary>
        [DataMember]
        public string iconSource { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        [DataMember]
        public string instruction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the question type description.
        /// </summary>
        [DataMember]
        public string questionTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the question type order.
        /// </summary>
        [DataMember]
        public int? questionTypeOrder { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public int questionTypeId { get; set; }

        #endregion
    }
}