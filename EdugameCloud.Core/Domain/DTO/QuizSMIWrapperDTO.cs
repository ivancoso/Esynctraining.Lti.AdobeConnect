namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz smi wrapper dto.
    /// </summary>
    [DataContract]
    [KnownType(typeof(QuizDTO))]
    [KnownType(typeof(SubModuleItemDTO))]
    public class QuizSMIWrapperDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz dto.
        /// </summary>
        [DataMember]
        public QuizDTO QuizDTO { get; set; }

        /// <summary>
        /// Gets or sets the smi dto.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiDTO { get; set; }

        #endregion
    }
}