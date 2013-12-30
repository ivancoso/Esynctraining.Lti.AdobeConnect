namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The quiz dto.
    /// </summary>
    [DataContract]
    public class QuizDTO
    {
        #region Constructors and Destructors

        public QuizDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public QuizDTO(Quiz result)
        {
            this.quizId = result.Id;
            this.subModuleItemId = result.SubModuleItem.Return(x => x.Id, (int?)null);
            this.scoreTypeId = result.ScoreType.Return(x => x.Id, (int?)null);
            this.quizFormatId = result.QuizFormat.Return(x => x.Id, (int?)null);
            this.description = result.Description;
            this.quizName = result.QuizName;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public virtual string description { get; set; }

        /// <summary>
        ///     Gets or sets the quiz format.
        /// </summary>
        [DataMember]
        public virtual int? quizFormatId { get; set; }

        /// <summary>
        ///     Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public virtual string quizName { get; set; }

        /// <summary>
        ///     Gets or sets the score type.
        /// </summary>
        [DataMember]
        public virtual int? scoreTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public virtual int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public virtual int quizId { get; set; }

        #endregion
    }
}