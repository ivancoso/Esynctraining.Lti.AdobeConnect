namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The paged distractor DTO.
    /// </summary>
    [DataContract]
    public class PagedDistractorDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        [DataMember]
        public DistractorDTO[] objects { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion
    }
}