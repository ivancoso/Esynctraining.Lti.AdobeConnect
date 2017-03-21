namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The quiz DTO.
    /// </summary>
    [DataContract]
    public class QuizDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizDTO"/> class.
        /// </summary>
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
//            this.scoreTypeId = result.ScoreType.Return(x => x.Id, (int?)null); //not used
            this.quizFormatId = result.QuizFormat.Return(x => x.Id, (int?)null);
            this.description = result.Description;
            this.quizName = result.QuizName;
            this.lmsQuizId = result.LmsQuizId ?? 0;
            this.isPostQuiz = result.IsPostQuiz;
            PassingScore = result.PassingScore;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the quiz format.
        /// </summary>
        [DataMember]
        public int? quizFormatId { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string quizName { get; set; }

        // <summary>
        // Gets or sets the score type.
        // </summary>
//        [DataMember]
//        public int? scoreTypeId { get; set; } // not used

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int quizId { get; set; }

        /// <summary>
        /// Gets or sets the moodle id.
        /// </summary>
        [DataMember]
        public int lmsQuizId { get; set; }
        
        [DataMember]
        public bool isPostQuiz { get; set; }

        [DataMember(Name = "passingScore")]
        public int PassingScore { get; set; }

        #endregion
    }
}