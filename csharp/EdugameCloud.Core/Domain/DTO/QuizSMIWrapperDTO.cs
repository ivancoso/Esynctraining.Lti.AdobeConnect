namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz SMI wrapper DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(QuizDTO))]
    [KnownType(typeof(SubModuleItemDTO))]
    public class QuizSMIWrapperDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz DTO.
        /// </summary>
        [DataMember]
        public QuizDTO QuizDTO { get; set; }

        /// <summary>
        /// Gets or sets the SMI DTO.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiDTO { get; set; }

        #endregion
    }
}