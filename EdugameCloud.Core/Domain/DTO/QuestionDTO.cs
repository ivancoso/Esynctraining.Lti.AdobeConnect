namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The question DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FileDTO))]
    [KnownType(typeof(DistractorDTO))]
    public class QuestionDTO
    {
        /// <summary>
        /// The distractors field.
        /// </summary>
        private DistractorDTO[] distractorsField = { };

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuestionDTO" /> class.
        /// </summary>
        public QuestionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionDTO"/> class.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public QuestionDTO(Question question, QuestionFor instance)
        {
            this.questionId = question.Id;
            this.question = question.QuestionName;
            this.questionTypeId = question.QuestionType.Id;
            this.questionOrder = question.QuestionOrder;
            this.subModuleItemId = question.SubModuleItem.With(x => x.Id);
            this.imageId = question.Image.Return(x => x.Id, (Guid?)null);
            this.instruction = question.Instruction;
            this.correctMessage = question.CorrectMessage;
            this.incorrectMessage = question.IncorrectMessage;
            this.hint = question.Hint;
            this.createdBy = question.CreatedBy.Return(x => x.Id, (int?)null);
            this.modifiedBy = question.ModifiedBy.Return(x => x.Id, (int?)null);
            this.dateCreated = question.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.dateModified = question.DateModified.With(x => x.ConvertToUnixTimestamp());
            this.isActive = question.IsActive;
            this.scoreValue = question.ScoreValue;
            this.imageVO = question.Image.Return(x => new FileDTO(x), null);
            this.distractors = question.Distractors.Select(x => new DistractorDTO(x)).ToArray();
            this.correctReference = question.CorrectReference;
            this.randomizeAnswers = question.RandomizeAnswers;

            this.FillCustomProperties(instance);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether allow other.
        /// </summary>
        [DataMember]
        public bool allowOther { get; set; }

        /// <summary>
        /// Gets or sets the answer option type.
        /// </summary>
        [DataMember]
        public int? answerOptionType { get; set; }

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        [DataMember]
        public string correctMessage { get; set; }

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        [DataMember]
        public string correctReference { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public DistractorDTO[] distractors
        {
            get
            {
                return this.distractorsField ?? new DistractorDTO[] { };
            }

            set
            {
                this.distractorsField = value;
            }
        }

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
        /// Gets or sets the is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether question is mandatory.
        /// </summary>
        [DataMember]
        public bool isMandatory { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        [DataMember]
        public int? pageNumber { get; set; }

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
        /// Gets or sets the question order.
        /// </summary>
        [DataMember]
        public int questionOrder { get; set; }

        /// <summary>
        /// Gets or sets the question type id.
        /// </summary>
        [DataMember]
        public int questionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// </summary>
        [DataMember]
        public string restrictions { get; set; }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        [DataMember]
        public int scoreValue { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        [DataMember]
        public decimal? totalWeightBucket { get; set; }

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        [DataMember]
        public int? weightBucketType { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether randomize answers.
        /// </summary>
        [DataMember]
        public bool? randomizeAnswers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The fill custom properties.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private void FillCustomProperties(QuestionFor instance)
        {
            if (instance != null)
            {
                foreach (QuestionTypeEnum questionType in instance.QuestionTypes)
                {
                    switch (questionType)
                    {
                        case QuestionTypeEnum.Rate:
                            var qr = (QuestionForRate)instance;
                            this.restrictions = qr.Restrictions;
                            this.allowOther = qr.AllowOther.HasValue && qr.AllowOther.Value;
                            break;
                        case QuestionTypeEnum.RateScaleLikert:
                            var ql = (QuestionForLikert)instance;
                            this.allowOther = ql.AllowOther.HasValue && ql.AllowOther.Value;
                            break;
                        case QuestionTypeEnum.OpenAnswerMultiLine:
                        case QuestionTypeEnum.OpenAnswerSingleLine:
                            var qoa = (QuestionForOpenAnswer)instance;
                            this.restrictions = qoa.Restrictions;
                            break;
                        case QuestionTypeEnum.WeightedBucketRatio:
                            var qwb = (QuestionForWeightBucket)instance;
                            this.totalWeightBucket = qwb.TotalWeightBucket;
                            this.weightBucketType = qwb.WeightBucketType;
                            this.allowOther = qwb.AllowOther.HasValue && qwb.AllowOther.Value;
                            break;
                        case QuestionTypeEnum.SingleMultipleChoiceImage:
                        case QuestionTypeEnum.SingleMultipleChoiceText:
                            var qs = (QuestionForSingleMultipleChoice)instance;
                            this.allowOther = qs.AllowOther.HasValue && qs.AllowOther.Value;
                            this.restrictions = qs.Restrictions;
                            break;
                    }
                }

                this.pageNumber = instance.PageNumber;
                this.isMandatory = instance.IsMandatory;
            }
        }

        #endregion
    }
}