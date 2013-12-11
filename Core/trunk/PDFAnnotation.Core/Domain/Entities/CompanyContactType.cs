namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The company contact type.
    /// </summary>
    public class CompanyContactType : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets CompanyContactTypeName
        /// </summary>
        public virtual string CompanyContactTypeName { get; set; }

        #endregion
    }
}