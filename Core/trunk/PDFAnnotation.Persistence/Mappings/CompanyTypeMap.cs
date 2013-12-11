namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The firm type mapping
    /// </summary>
    public class CompanyTypeMap : BaseClassMap<CompanyType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyTypeMap"/> class.
        /// </summary>
        public CompanyTypeMap()
        {
            this.Map(x => x.CompanyTypeName).Length(255).Not.Nullable();
        }

        #endregion
    }
}