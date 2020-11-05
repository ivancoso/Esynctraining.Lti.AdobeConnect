// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyContactDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The companycontact data transfer object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The companycontact data transfer object.
    /// </summary>
    [DataContract]
    [Serializable]
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
        /// <param name="cc">
        /// The CompanyContact.
        /// </param>
        public CompanyContactDTO(CompanyContact cc)
        {
            if (cc != null)
            {
                this.companyId = cc.Company.With(x => x.Id);
                this.contactTypeId = cc.ContactType.With(x => x.Id);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the contact type id.
        /// </summary>
        [DataMember]
        public int contactTypeId { get; set; }

        #endregion
    }
}