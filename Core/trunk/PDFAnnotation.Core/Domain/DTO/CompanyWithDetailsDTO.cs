namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The company DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(CompanyContactDTO))]
    [KnownType(typeof(CategoryDTO))]
    [KnownType(typeof(CompanyDTO))]
    public class CompanyWithDetailsDTO : CompanyDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyWithDetailsDTO" /> class.
        /// </summary>
        public CompanyWithDetailsDTO()
        {
            this.firmContactsTabFirstPage = new TabFirstPageWrapper<CompanyContactExDTO>();
            this.casesTabFirstPage = new TabFirstPageWrapper<CategoryDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyWithDetailsDTO"/> class. 
        /// </summary>
        /// <param name="f">
        /// The c.
        /// </param>
        /// <param name="firmContacts">
        /// The company Contacts.
        /// </param>
        /// <param name="cases">
        /// The categories.
        /// </param>
        public CompanyWithDetailsDTO(Company f, TabFirstPageWrapper<CompanyContact> firmContacts = null, TabFirstPageWrapper<Category> cases = null) : base(f)
        {
                if (firmContacts != null)
                {
                    this.firmContactsTabFirstPage = firmContacts.Convert(x => new CompanyContactExDTO(x));
                }

                if (cases != null)
                {
                    this.casesTabFirstPage = cases.Convert(x => new CategoryDTO(x));
                }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the categories first page.
        /// </summary>
        [DataMember]
        public TabFirstPageWrapper<CategoryDTO> casesTabFirstPage { get; set; }

        /// <summary>
        /// Gets or sets the company contacts first page.
        /// </summary>
        [DataMember]
        public TabFirstPageWrapper<CompanyContactExDTO> firmContactsTabFirstPage { get; set; }

        #endregion
    }
}