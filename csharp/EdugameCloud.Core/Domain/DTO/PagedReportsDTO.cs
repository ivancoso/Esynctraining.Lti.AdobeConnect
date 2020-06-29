namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The paged reports items DTO.
    /// </summary>
    [DataContract]
    public class PagedReportsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        [DataMember]
        public ReportDTO[] objects { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember]
        public int totalCount { get; set; }

        #endregion
    }
}