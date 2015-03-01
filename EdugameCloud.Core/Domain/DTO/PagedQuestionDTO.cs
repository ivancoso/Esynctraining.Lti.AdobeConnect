namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The paged question DTO.
    /// </summary>
    [DataContract]
    public class PagedQuestionDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        [DataMember]
        public QuestionDTO[] objects { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion
    }
}