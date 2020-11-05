using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Persistence.Mappings.CompanyContactMapping
{
    /// <summary>
    /// The country mapping
    /// </summary>
    public class CountryMap : BaseClassMap<Country>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryMap"/> class.
        /// </summary>
        public CountryMap()
        {
            this.Map(x => x.CountryName).Length(255).Nullable();
            this.Map(x => x.CountryCode).Length(3).Nullable();
            this.Map(x => x.CountryCode3).Length(4).Nullable();
        }

    }

}