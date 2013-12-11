namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The firm contact.
    /// </summary>
    public class CompanyContact : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets ContactId
        /// </summary>
        public virtual Contact Contact { get; set; }

        /// <summary>
        ///     Gets or sets CompanyContactType
        /// </summary>
        public virtual CompanyContactType CompanyContactType { get; set; }

        /// <summary>
        ///     Gets or sets Company
        /// </summary>
        public virtual Company Company { get; set; }

        #endregion
    }
}