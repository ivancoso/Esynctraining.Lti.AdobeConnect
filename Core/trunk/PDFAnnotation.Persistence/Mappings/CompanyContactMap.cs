namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContact Fluent NHibernate mapping class.
    /// </summary>
    public class CompanyContactMap : BaseClassMap<CompanyContact>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyContactMap" /> class.
        /// </summary>
        public CompanyContactMap()
        {
            this.References(x => x.Company).Not.Nullable();
            this.References(x => x.Contact).Not.Nullable();
            this.References(x => x.ContactType).Not.Nullable();
        }

        #endregion
    }
}