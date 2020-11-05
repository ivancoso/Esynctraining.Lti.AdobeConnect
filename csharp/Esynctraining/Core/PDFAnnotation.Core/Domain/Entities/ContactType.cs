namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The contact type.
    /// </summary>
    public class ContactType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the contact type name.
        /// </summary>
        public virtual string ContactTypeName { get; set; }

        /// <summary>
        /// Gets or sets the ac type name.
        /// </summary>
        public virtual string ACMappedType { get; set; }

        #endregion
    }
}