namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The contact DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ContactDTO))]
    [KnownType(typeof(CategoryDTO))]
    [Serializable]
    public class ContactWithDetailsDTO : ContactDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactWithDetailsDTO" /> class.
        /// </summary>
        public ContactWithDetailsDTO()
        {
            this.casesTabFirstPage = new TabFirstPageWrapper<CategoryDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactWithDetailsDTO"/> class.
        /// </summary>
        /// <param name="cnt">
        /// The user.
        /// </param>
        /// <param name="cases">
        /// The categories.
        /// </param>
        /// <param name="notes">
        /// The notes.
        /// </param>
        /// <param name="emailHistory">
        /// The email History.
        /// </param>
        public ContactWithDetailsDTO(Contact cnt, TabFirstPageWrapper<Category> cases = null) : base(cnt)
        {
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

        #endregion

    }
}