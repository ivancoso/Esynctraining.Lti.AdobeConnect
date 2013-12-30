namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The question for admin dto.
    /// </summary>
    [DataContract]
    public class QuestionForAdminDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        [DataMember]
        public List<DistractorDTO> distractors { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public List<int> questionResultIds { get; set; }

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
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public virtual int questionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the correct answer count.
        /// </summary>
        [DataMember]
        public virtual int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public virtual string questionTypeName { get; set; }

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
        /// Gets or sets the correct count.
        /// </summary>
        [DataMember]
        public virtual int correctCount
        {
            get
            {
                return this.distractors.With(x => x.Count(d => d.isCorrect == true));
            }
            set
            {
                
            }
        }

        /// <summary>
        /// Gets or sets the incorrect count.
        /// </summary>
        [DataMember]
        public virtual int incorrectCount
        {
            get
            {
                return this.distractors.With(x => x.Count() - this.correctCount);
            }
            set
            {

            }
        }

        #endregion
    }
}