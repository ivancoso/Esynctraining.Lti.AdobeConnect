namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The question from stored procedure DTO.
    /// </summary>
    [DataContract]
    public class QuestionFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        [DataMember]
        public virtual string correctMessage { get; set; }

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        [DataMember]
        public virtual string correctReference { get; set; }

        /// <summary>
        /// Gets or sets the total weight bucket.
        /// </summary>
        [DataMember]
        public virtual decimal? totalWeightBucket { get; set; }

        /// <summary>
        /// Gets or sets the weight bucket type.
        /// </summary>
        [DataMember]
        public virtual int? weightBucketType { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// </summary>
        [DataMember]
        public virtual string restrictions { get; set; }

        /// <summary>
        /// Gets or sets the allow other.
        /// </summary>
        [DataMember]
        public virtual bool? allowOther { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is mandatory.
        /// </summary>
        [DataMember]
        public virtual bool isMandatory { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [DataMember]
        public virtual int? height { get; set; }

        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        [DataMember]
        public virtual string hint { get; set; }

        /// <summary>
        /// Gets or sets the File id.
        /// </summary>
        [DataMember]
        public virtual Guid? imageId { get; set; }

        /// <summary>
        /// Gets or sets the image vo.
        /// </summary>
        [DataMember]
        public virtual FileDTO imageVO { get; set; }

        /// <summary>
        /// Gets or sets the incorrect message.
        /// </summary>
        [DataMember]
        public virtual string incorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        [DataMember]
        public virtual string instruction { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        [DataMember]
        public virtual string question { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public virtual int questionId { get; set; }

        /// <summary>
        /// Gets or sets the question order.
        /// </summary>
        [DataMember]
        public virtual int questionOrder { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        [DataMember]
        public virtual int? pageNumber { get; set; }

        /// <summary>
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public virtual int questionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        [DataMember]
        public virtual int scoreValue { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [DataMember]
        public virtual int? width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        [DataMember]
        public virtual int? x { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        [DataMember]
        public virtual int? y { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether randomize answers.
        /// </summary>
        [DataMember]
        public virtual bool? randomizeAnswers { get; set; }

        /// <summary>
        /// Gets or sets the is always rate dropdown.
        /// </summary>
        [DataMember]
        public virtual bool? isAlwaysRateDropdown { get; set; }

        #endregion
    }
}