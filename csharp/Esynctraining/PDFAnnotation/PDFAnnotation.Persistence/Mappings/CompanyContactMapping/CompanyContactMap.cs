using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Persistence.Mappings.CompanyContactMapping
{
    /// <summary>
    /// The CompanyContact Fluent NHibernate mapping class.
    /// </summary>
    public class CompanyContactMap : BaseClassMap<CompanyContact>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactMap" /> class.
        /// </summary>
        public CompanyContactMap()
        {
            this.References(x => x.Company).Not.Nullable();
            this.References(x => x.Contact).Not.Nullable();
            this.References(x => x.ContactType).Not.Nullable();
        }

    }

}