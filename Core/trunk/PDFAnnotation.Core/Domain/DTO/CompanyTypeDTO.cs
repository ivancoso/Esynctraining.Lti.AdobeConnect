namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The company type DTO.
    /// </summary>
    [DataContract]
    public class CompanyTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyTypeDTO" /> class.
        /// </summary>
        public CompanyTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyTypeDTO"/> class.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public CompanyTypeDTO(CompanyType c)
        {
            this.companyTypeId = c.Id;
            this.companyType = c.CompanyTypeName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the contact type id.
        /// </summary>
        [DataMember]
        public int companyTypeId { get; set; }

        /// <summary>
        /// Gets or sets the contact type.
        /// </summary>
        [DataMember]
        public string companyType { get; set; }


        #endregion
    }
}