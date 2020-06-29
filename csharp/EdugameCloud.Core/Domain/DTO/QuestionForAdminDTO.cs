namespace EdugameCloud.Core.Domain.DTO
{
    using System.Linq;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The question for admin DTO.
    /// </summary>
    [DataContract]
    public sealed class QuestionForAdminDTO
    {
        #region Public Properties

        [DataMember]
        public int questionOrder { get; set; }

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        [DataMember]
        public DistractorDTO[] distractors { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public int[] questionResultIds { get; set; }

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
        /// Gets or sets the correct answer count.
        /// </summary>
        [DataMember]
        public int correctAnswerCount { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public string questionTypeName { get; set; }

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
        /// Gets or sets the correct count.
        /// </summary>
        [DataMember]
        public int correctCount
        {
            get
            {
                return this.distractors.With(x => x.Count(d => d.isCorrect == true));
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the incorrect count.
        /// </summary>
        [DataMember]
        public int incorrectCount
        {
            get
            {
                return this.distractors.With(x => x.Count() - this.correctCount);
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        #endregion
    }
}