namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The company type.
    /// </summary>
    public class CompanyType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the company type name.
        /// </summary>
        public virtual string CompanyTypeName { get; set; }

        #endregion
    }
}