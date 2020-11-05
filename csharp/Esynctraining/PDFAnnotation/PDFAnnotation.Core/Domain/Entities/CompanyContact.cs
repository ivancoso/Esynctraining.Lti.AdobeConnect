namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The contact.
    /// </summary>
    public class CompanyContact : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual Company Company { get; set; }

        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        public virtual Contact Contact { get; set; }

        /// <summary>
        /// Gets or sets the contact type.
        /// </summary>
        public virtual ContactType ContactType { get; set; }

        #endregion
    }
}