namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The quiz format DTO.
    /// </summary>
    [DataContract]
    public class QuizFormatDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFormatDTO"/> class.
        /// </summary>
        public QuizFormatDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFormatDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public QuizFormatDTO(QuizFormat result)
        {
            this.quizFormatId = result.Id;
            this.quizFormatName = result.QuizFormatName;
            this.dateCreated = result.DateCreated.ConvertToUnixTimestamp();
            this.isActive = result.IsActive;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string quizFormatName { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int quizFormatId { get; set; }

        #endregion
    }
}