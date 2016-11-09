namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The question from stored procedure DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FileDTO))]
    public sealed class QuestionFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        [DataMember]
        public string correctMessage { get; set; }

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        [DataMember]
        public string correctReference { get; set; }

        /// <summary>
        /// Gets or sets the total weight bucket.
        /// </summary>
        [DataMember]
        public decimal? totalWeightBucket { get; set; }

        /// <summary>
        /// Gets or sets the weight bucket type.
        /// </summary>
        [DataMember]
        public int? weightBucketType { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// </summary>
        [DataMember]
        public string restrictions { get; set; }

        /// <summary>
        /// Gets or sets the allow other.
        /// </summary>
        [DataMember]
        public bool? allowOther { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is mandatory.
        /// </summary>
        [DataMember]
        public bool isMandatory { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [DataMember]
        public int? height { get; set; }

        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        [DataMember]
        public string hint { get; set; }

        /// <summary>
        /// Gets or sets the File id.
        /// </summary>
        [DataMember]
        public Guid? imageId { get; set; }

        /// <summary>
        /// Gets or sets the image vo.
        /// </summary>
        [DataMember]
        public FileDTO imageVO { get; set; }

        /// <summary>
        /// Gets or sets the incorrect message.
        /// </summary>
        [DataMember]
        public string incorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        [DataMember]
        public string instruction { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        [DataMember]
        public string question { get; set; }

        /// <summary>
        /// Gets or sets html of a question.
        /// </summary>
        [DataMember]
        public string htmlText { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public int questionId { get; set; }

        /// <summary>
        /// Gets or sets the question order.
        /// </summary>
        [DataMember]
        public int questionOrder { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        [DataMember]
        public int? pageNumber { get; set; }

        /// <summary>
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public int questionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        [DataMember]
        public int scoreValue { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [DataMember]
        public int? width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        [DataMember]
        public int? x { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        [DataMember]
        public int? y { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether randomize answers.
        /// </summary>
        [DataMember]
        public bool? randomizeAnswers { get; set; }

        /// <summary>
        /// Gets or sets the is always rate dropdown.
        /// </summary>
        [DataMember]
        public bool? isAlwaysRateDropdown { get; set; }

        /// <summary>
        /// Gets or sets the is always rate dropdown.
        /// </summary>
        [DataMember]
        public int rows { get; set; }

        /// <summary>
        /// TODO: make different types for single and multiple choice questions, remove this property
        /// Currently is needed for Moodle (single choice which can have multiple correct answers)
        /// </summary>
        [DataMember]
        public bool isMultipleChoice { get { return restrictions != null && restrictions.Contains("multi_choice"); } }

        #endregion
    }
}