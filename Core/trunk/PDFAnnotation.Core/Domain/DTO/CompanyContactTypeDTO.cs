namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContactType data transfer object.
    /// </summary>
    [DataContract]
    public class CompanyContactTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyContactTypeDTO" /> class.
        /// </summary>
        public CompanyContactTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactTypeDTO"/> class.
        /// </summary>
        /// <param name="companyContactType">
        /// The CompanyContactType.
        /// </param>
        public CompanyContactTypeDTO(CompanyContactType companyContactType)
        {
            if (companyContactType != null)
            {
                this.companyContactTypeId = companyContactType.Id;
                this.companyContactType = companyContactType.CompanyContactTypeName;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets CompanyContactType Id
        /// </summary>
        [DataMember]
        public int companyContactTypeId { get; set; }

        /// <summary>
        ///     Gets or sets CompanyContactType Name
        /// </summary>
        [DataMember]
        public string companyContactType { get; set; }

        #endregion
    }
}