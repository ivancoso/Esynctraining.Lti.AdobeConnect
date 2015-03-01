namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The paged quiz DTO.
    /// </summary>
    [DataContract]
    public class PagedQuizDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        [DataMember]
        public QuizDTO[] objects { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion
    }
}