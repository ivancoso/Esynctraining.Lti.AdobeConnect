namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The contact DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(CategoryDTO))]
    [Serializable]
    public class ContactDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDTO" /> class.
        /// </summary>
        public ContactDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactDTO"/> class.
        /// </summary>
        /// <param name="cnt">
        /// The user.
        /// </param>
        public ContactDTO(Contact cnt)
        {
            if (cnt != null)
            {
                this.rbContactId = cnt.RBContactId;
                this.contactId = cnt.Id;
                this.dateCreated = cnt.DateCreated;
                this.dateModified = cnt.DateModified ?? cnt.DateCreated;
                this.email = cnt.Email;
                this.firstName = cnt.FirstName;
                this.lastName = cnt.LastName;
                this.isActive = cnt.Status == ContactStatusEnum.Active;
                this.contactTypeId = cnt.ContactType.With(x => x.Id);
                this.officePhone = cnt.OfficePhone;
                this.mobilePhone = cnt.MobilePhone;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the case id.
        /// </summary>
        [DataMember]
        public int? rbContactId { get; set; }

        /// <summary>
        ///     Gets or sets the phone cell.
        /// </summary>
        [DataMember]
        public string mobilePhone { get; set; }

        /// <summary>
        ///     Gets or sets id
        /// </summary>
        [DataMember]
        public int contactId { get; set; }

        /// <summary>
        ///     Gets or sets the contact type id.
        /// </summary>
        [DataMember]
        public int contactTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        ///     Gets or sets the phone work.
        /// </summary>
        [DataMember]
        public string officePhone { get; set; }

        #endregion

    }
}