namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContactType Fluent NHibernate mapping class.
    /// </summary>
    public class CompanyContactTypeMap : BaseClassMap<CompanyContactType>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyContactTypeMap" /> class.
        /// </summary>
        public CompanyContactTypeMap()
        {
            this.Map(x => x.CompanyContactTypeName).Not.Nullable();
        }

        #endregion
    }
}