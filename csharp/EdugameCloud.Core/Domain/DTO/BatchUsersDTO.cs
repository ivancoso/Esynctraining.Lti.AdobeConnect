namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The batch users DTO.
    /// </summary>
    [DataContract]
    public class BatchUsersDTO
    {
        /// <summary>
        /// Gets or sets the batch type.
        /// "csv" || "xls" || "xlsx"
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the CSV or excel content.
        /// </summary>
        [DataMember]
        public string csvOrExcelContent { get; set; }

    }

}