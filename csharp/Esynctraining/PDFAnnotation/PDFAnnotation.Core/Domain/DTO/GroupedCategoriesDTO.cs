namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The grouped categories dto.
    /// </summary>
    [DataContract]
    public class GroupedCategoriesDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [DataMember]
        public CategoryDTO[] categories { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        #endregion
    }
}