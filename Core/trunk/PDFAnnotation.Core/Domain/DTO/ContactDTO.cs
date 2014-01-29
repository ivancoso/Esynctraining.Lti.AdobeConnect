namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The contact DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(CategoryDTO))]
    [KnownType(typeof(CompanyContactDTO))]
    [Serializable]
    public class ContactDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDTO" /> class.
        /// </summary>
        public ContactDTO()
        {
            this.companies = new List<CompanyContactDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactDTO"/> class.
        /// </summary>
        /// <param name="cnt">
        /// The user.
        /// </param>
        /// <param name="userToken">
        /// The user Token.
        /// </param>
        public ContactDTO(Contact cnt, string userToken = null)
        {
            this.companies = new List<CompanyContactDTO>();
            if (cnt != null)
            {
                this.rbContactId = cnt.RBContactId;
                this.companies = cnt.CompanyContacts.Select(x => new CompanyContactDTO(x)).ToList();
                this.contactId = cnt.Id;
                this.dateCreated = cnt.DateCreated;
                this.dateModified = cnt.DateModified ?? cnt.DateCreated;
                this.email = cnt.Email;
                this.firstName = cnt.FirstName;
                this.lastName = cnt.LastName;
                this.isActive = cnt.Status == ContactStatusEnum.Active;
                this.isSuperAdmin = cnt.IsSuperAdmin;
                this.officePhone = cnt.OfficePhone;
                this.mobilePhone = cnt.MobilePhone;
                this.userToken = userToken;
                this.isSuperAdmin = cnt.IsSuperAdmin;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the case id.
        /// </summary>
        [DataMember]
        public List<CompanyContactDTO> companies { get; set; }

        /// <summary>
        ///     Gets or sets id
        /// </summary>
        [DataMember]
        public int contactId { get; set; }

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
        /// Gets or sets a value indicating whether is super admin.
        /// </summary>
        [DataMember]
        public bool isSuperAdmin { get; set; }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        ///     Gets or sets the phone cell.
        /// </summary>
        [DataMember]
        public string mobilePhone { get; set; }

        /// <summary>
        ///     Gets or sets the phone work.
        /// </summary>
        [DataMember]
        public string officePhone { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        ///     Gets or sets the case id.
        /// </summary>
        [DataMember]
        public int? rbContactId { get; set; }

        /// <summary>
        ///     Gets or sets the user token.
        /// </summary>
        [DataMember]
        public string userToken { get; set; }

        #endregion
    }
}