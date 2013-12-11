namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The FirmContact data transfer object.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ContactDTO))]
    public class CompanyContactExDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyContactExDTO" /> class.
        /// </summary>
        public CompanyContactExDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactExDTO"/> class. 
        /// </summary>
        /// <param name="companyContact">
        /// The CompanyContact.
        /// </param>
        public CompanyContactExDTO(CompanyContact companyContact)
        {
            if (companyContact != null)
            {
                this.companyContactId = companyContact.Id;
                this.companyId = companyContact.Company.With(x => x.Id);
                this.contact = new ContactDTO(companyContact.Contact);
                this.companyContactTypeId = companyContact.CompanyContactType.With(x => x.Id);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ContactId
        /// </summary>
        [DataMember]
        public ContactDTO contact { get; set; }

        /// <summary>
        ///     Gets or sets FirmContactId
        /// </summary>
        [DataMember]
        public int companyContactId { get; set; }

        /// <summary>
        ///     Gets or sets FirmContactTypeId
        /// </summary>
        [DataMember]
        public int companyContactTypeId { get; set; }

        /// <summary>
        ///     Gets or sets FirmId
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        #endregion
    }
}