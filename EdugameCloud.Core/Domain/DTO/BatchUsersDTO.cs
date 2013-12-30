namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The batch users dto.
    /// </summary>
    [DataContract]
    public class BatchUsersDTO
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the batch type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the csv or excel content.
        /// </summary>
        [DataMember]
        public string csvOrExcelContent { get; set; }

        #endregion
    }
}