namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContact data transfer object.
    /// </summary>
    [DataContract]
    public class CompanyContactDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyContactDTO" /> class.
        /// </summary>
        public CompanyContactDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactDTO"/> class.
        /// </summary>
        /// <param name="companyContact">
        /// The CompanyContact.
        /// </param>
        public CompanyContactDTO(CompanyContact companyContact)
        {
            if (companyContact != null)
            {
                this.companyContactId = companyContact.Id;
                this.companyId = companyContact.Company.With(x => x.Id);
                this.contactId = companyContact.Contact.With(x => x.Id);
                this.companyContactTypeId = companyContact.CompanyContactType.With(x => x.Id);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ContactId
        /// </summary>
        [DataMember]
        public virtual int contactId { get; set; }

        /// <summary>
        ///     Gets or sets FirmContactId
        /// </summary>
        [DataMember]
        public virtual int companyContactId { get; set; }

        /// <summary>
        ///     Gets or sets FirmContactTypeId
        /// </summary>
        [DataMember]
        public virtual int companyContactTypeId { get; set; }

        /// <summary>
        ///     Gets or sets FirmId
        /// </summary>
        [DataMember]
        public virtual int companyId { get; set; }

        #endregion
    }
}