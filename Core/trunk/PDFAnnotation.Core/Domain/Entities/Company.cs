// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Company.cs" company="">
//   
// </copyright>
// <summary>
//   The company.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PDFAnnotation.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The company.
    /// </summary>
    public class Company : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the color primary.
        /// </summary>
        public virtual string ColorPrimary { get; set; }

        /// <summary>
        /// Gets or sets the color secondary.
        /// </summary>
        public virtual string ColorSecondary { get; set; }

        /// <summary>
        /// Gets or sets the color text.
        /// </summary>
        public virtual string ColorText { get; set; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the logo.
        /// </summary>
        public virtual File Logo { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        ///     Gets or sets the rb company id.
        /// </summary>
        public virtual int? RBFirmId { get; set; }

        #endregion
    }
}