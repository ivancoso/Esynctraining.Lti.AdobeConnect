namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;


    /// <summary>
    ///     The quiz format dto.
    /// </summary>
    [DataContract]
    public class QuizFormatDTO
    {
        #region Constructors and Destructors

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
            this.dateCreated = result.DateCreated;
            this.isActive = result.IsActive;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public virtual string quizFormatName { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public virtual bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public virtual int quizFormatId { get; set; }

        #endregion
    }
}